document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById('members-sidebar');
    const toggleBtn = document.getElementById('toggle-members-sidebar');
    const toggleIcon = document.getElementById('toggle-icon');

    let isSidebarOpen = false;

    //Ẩn hiện Sidebar thành viên
    if (toggleBtn && sidebar && toggleIcon) {
        toggleBtn.addEventListener('click', function () {
            if (isSidebarOpen) {
                sidebar.style.right = '-300px';
                toggleBtn.style.right = '0';
                toggleIcon.className = 'bi bi-people-fill fs-5';
            } else {
                sidebar.style.right = '0';
                toggleBtn.style.right = '280px';
                toggleIcon.className = 'bi bi-chevron-right fs-5';
            }
            isSidebarOpen = !isSidebarOpen;
        });
    }

    $(document).ready(function () {
        //Xử lý gửi lời mời thêm thành viên nhóm.
        $("#btn-send-invite").click(function () {
            const email = $("#invite-email").val().trim();
            const groupId = document.getElementById("groupIdHidden").value;

            if (email === "") {
                toastr.warning("Vui lòng nhập email.");
                return;
            }

            $.ajax({
                type: "POST",
                url: "/Group/InviteMember",
                data: { groupId: groupId, email: email },
                success: function () {
                    $('#inviteModal').modal('hide');
                    toastr.success("Đã gửi lời mời.");
                },
                error: function () {
                    toastr.error("Gửi lời mời thất bại.");
                }
            });
        });
    });

    //Hiện Model Thêm Task
    $("#addTaskButton").click(function () {
        $("#addTaskModal").modal("show");
    });

    //Thêm task
    $("#saveNewTaskBtn").click(function () {
        const title = $("#newTaskTitle").val().trim();
        const groupId = document.getElementById("groupIdHidden").value;

        if (title === "") {
            toastr.warning("Vui lòng nhập tên mục.");
            return;
        }

        $.ajax({
            type: "POST",
            url: "/Task/AddGroupTask",
            data: { groupId: groupId, title: title },
            success: function (response) {
                $("#addTaskModal").modal("hide");
                $("#newTaskTitle").val("");

                // Cập nhật lại select trong Modal SubTask
                $("#taskSelect").html(response); // `response` là PartialView danh sách Task

                toastr.success("Thêm mục thành công.");
            },
            error: function () {
                toastr.error("Thêm mục thất bại.");
            }
        });
    });

    //Hiển thị Modal thêm Subtask
    $("#btnAddSubTask").click(function () {
        $("#addSubTaskModal").modal("show");
    });

    // 👉 Xử lý khi nhấn Thêm SubTask trong Modal
    $("#saveNewSubTaskBtn").click(function () {
        const title = $("#subTaskTitle").val().trim();        // Đúng id
        const taskId = $("#taskSelect").val();
        const dueDate = $("#subTaskDueDate").val();           // Đúng id

        if (title === "") {
            toastr.warning("Vui lòng nhập tên công việc.");
            return;
        }

        $.ajax({
            type: "POST",
            url: "/Task/AddGroupSubTask",
            data: {
                taskId: taskId,
                title: title,
                dueDate: dueDate
            },
            success: function (response) {
                $("#addSubTaskModal").modal("hide");

                // Reset form
                $("#subTaskTitle").val("");
                $("#subTaskDueDate").val("");

                // Thêm SubTask mới vào giao diện
                $(".task-table").append(response);

                toastr.success("Thêm công việc thành công.");
            },
            error: function () {
                toastr.error("Thêm công việc thất bại.");
            }
        });
    });
    $(document).on("click", ".more-task-info", function () {
        const taskId = $(this).data("task-id");
        const groupId = $(this).data("group-id");

        $.ajax({
            url: "/Task/GetTaskDetailModal",
            type: "GET",
            data: { taskId: taskId, groupId: groupId },
            success: function (response) {
                $("#taskInfoContent").html(response);
                $("#taskInfoModal").modal("show");
                initDueDateFlatpickr();
            },
            error: function () {
                toastr.error("Không thể tải thông tin công việc.");
            }
        });
    });
});

//Hiển thị danh sách thành viên để phân công
$(document).on("click", ".assign-toggle", function () {
    const subTaskId = $(this).data("subtask-id");
    const container = $("#assignPopup-" + subTaskId);

    // Toggle hiển thị
    if (container.children().length > 0) {
        container.empty();
        return;
    }

    $.get("/Task/GetAssignPopup", { subTaskId: subTaskId }, function (html) {
        $(".assign-popup-container").empty(); // Đóng popup khác nếu có
        container.html(html);
    });
});

// Tìm kiếm thành viên trong popup
$(document).on("input", ".search-member", function () {
    const keyword = $(this).val().toLowerCase();
    const items = $(this).closest(".card").find(".assign-item");

    items.each(function () {
        const username = $(this).text().toLowerCase();
        $(this).toggle(username.includes(keyword));
    });
});

// Thực hiện thay đổi phân công
$(document).on('click', '.assign-item', function () {
    const $item = $(this);
    const userId = $item.data('user-id');

    // Tìm container chứa assign để lấy subTaskId phù hợp cả trong List và Detail
    const $container = $item.closest('.assign-popup-container, .list-assign-popup-container');

    let subTaskId = $container.attr('id')?.replace('assignPopup-', '').replace('listAssignPopup-', '');

    // Nếu không có id thì fallback tìm data-subtask-id gần nhất
    if (!subTaskId) {
        subTaskId = $item.closest('[data-subtask-id]').data('subtask-id');
    }

    $.ajax({
        type: 'POST',
        url: '/Task/ToggleAssignment',
        data: { subTaskId, userId },
        success: function (res) {
            if (res.isAssigned) {
                $item.addClass('bg-light');

                if ($item.find('.unassign-icon').length === 0) {
                    $item.append('<span class="text-danger ms-2 unassign-icon" title="Huỷ phân công" style="font-size: 1rem;">×</span>');
                }
            } else {
                $item.removeClass('bg-light');
                $item.find('.unassign-icon').remove();
            }
            updateAssignedAvatarsDetail(subTaskId, res.assignedUsers);
        },
        error: function () {
            alert('Lỗi khi phân công người dùng.');
        }
    });
});

// Thêm / xóa icon User trong phân công
function updateAssignedAvatarsDetail(subTaskId, users) {
    const avatarContainer = $(`.assigned-avatar-container[data-subtask-id="${subTaskId}"]`);

    if (!avatarContainer.length) return;

    // Xóa toàn bộ avatar và dấu "..."
    avatarContainer.empty();

    const maxToShow = 3;
    const usersToShow = users.slice(0, maxToShow);
    const remaining = users.length - maxToShow;

    // Thêm avatar
    usersToShow.forEach(user => {
        const img = $('<img>').attr({
            src: user.profilePicture || '/images/default-avatar.jpg',
            class: 'rounded-circle me-1',
            title: user.username
        }).css({ width: '33px', height: '33px' });

        avatarContainer.append(img);
    });

    // Nếu dư người thì hiển thị dấu "..."
    if (remaining > 0) {
        const moreIcon = $(`
            <div class="rounded-circle bg-secondary text-white d-flex justify-content-center align-items-center me-1"
                 style="width: 33px; height: 33px; font-size: 0.8rem;" title="+${remaining} người">
                ...
            </div>
        `);
        avatarContainer.append(moreIcon);
    }
}

//Click ra ngoài để đóng popup phân công
$(document).on("click", function (e) {
    // Nếu click không nằm trong popup và không phải nút toggle thì đóng popup
    if (!$(e.target).closest(".assign-popup-container").length &&
        !$(e.target).closest(".assign-toggle").length) {
        $(".assign-popup-container").empty();
    }
});

// ẩn tên công việc và hiển thị ô input để chỉnh sửa
$(document).on('click', '#task-title-display', function () {
    $(this).hide();                          // Ẩn phần hiển thị
    $('#task-title-edit').removeClass('d-none');  // Hiện form chỉnh sửa
    $('#task-title-input').focus();          // Focus vào input
});

// Ẩn phần input chỉnh sửa và hiển thị tên công việc
$(document).on('click', '#cancel-task-title', function () {
    $('#task-title-edit').addClass('d-none');     // Ẩn form chỉnh sửa
    $('#task-title-display').show();              // Hiện lại phần tên
});

//Thay đổi tên công việc khi ấn lưu
$(document).on('click', '#save-task-title', function () {
    const newTitle = $('#task-title-input').val().trim();
    const subTaskId = $('input[name="SubTaskId"]').val(); // Lấy từ hidden input

    if (newTitle.length === 0) {
        alert("Tên công việc không được để trống.");
        return;
    }

    $.ajax({
        url: '/Task/UpdateSubTask', // đúng route bạn đã có
        type: 'POST',
        data: {
            id: subTaskId,
            title: newTitle,
            type : 'title'
        },
        success: function () {
            // Cập nhật tên trong modal
            $('#task-title-display span').text(newTitle);
            $('#task-title-edit').addClass('d-none');
            $('#task-title-display').show();

            // Cập nhật tên ngoài danh sách nếu có
            $('#subtask-title-' + subTaskId).text(newTitle);
        },
        error: function (xhr) {
            alert(xhr.responseText || 'Lỗi khi cập nhật tên công việc.');
        }
    });
});

// Xử lý thay đổi ngày đến hạn
$(document).on('change', 'select[name="TaskId"]', function () {
    const taskId = $(this).val();
    const taskTitle = $(this).find('option:selected').text().trim();
    const subTaskId = $('input[name="SubTaskId"]').val();

    $.ajax({
        url: '/Task/UpdateSubTask',
        type: 'POST',
        data: {
            id: subTaskId,
            title: taskId,
            type: 'parent'
        },
        success: function () {
            console.log(taskTitle);
            // Cập nhật tên trong khối danh sách ngoài giao diện
            $(`.task-title[data-subtask-id="${subTaskId}"]`).text(taskTitle);
        },
        error: function (xhr) {
            alert(xhr.responseText || 'Lỗi khi cập nhật thuộc mục.');
        }
    });
});

// Hiện lịch khi click icon
$(document).on("click", ".due-date-trigger", function () {
    $(".calendar-popup").addClass("d-none"); // Ẩn popup khác
    $(this).siblings(".calendar-popup").removeClass("d-none");
});

// Đóng lịch
$(document).on("click", ".btn-close-calendar", function () {
    $(this).closest(".calendar-popup").addClass("d-none");
});

// Lưu ngày đã chọn
$(document).on("click", ".btn-save-calendar", function () {
    const popup = $(this).closest(".calendar-popup");
    const input = popup.find(".flatpickr-input");
    const selectedDate = input.data("selected-date");
    const subTaskId = $(this).data("subtask-id");

    if (!selectedDate) {
        alert("Vui lòng chọn ngày.");
        return;
    }

    function formatDate(dateStr) {
        const date = new Date(dateStr);
        const day = ('0' + date.getDate()).slice(-2);
        const month = ('0' + (date.getMonth() + 1)).slice(-2);
        const year = date.getFullYear();
        return `${day}/${month}/${year}`;
    }

    $.ajax({
        url: '/Task/UpdateSubTask',
        method: 'POST',
        data: {
            id: subTaskId,
            title: selectedDate,
            type: 'due'
        },
        success: function () {
            popup.addClass("d-none");

            const formattedDate = formatDate(selectedDate);

            // ✅ Cập nhật trong phần chi tiết
            $(`.due-date-text[data-subtask-id="${subTaskId}"]`).text(formattedDate);

            // ✅ Cập nhật ngoài danh sách
            $(`.due-date-display[data-subtask-id="${subTaskId}"]`).text(formattedDate);
        },
        error: function () {
            alert("Lỗi khi cập nhật ngày đến hạn.");
        }
    });
});

//Ẩn / hiện Popup phân công thành viên
$(document).on('click', '.list-assign-toggle', function (e) {
    e.stopPropagation(); // Không lan ra ngoài để tránh đóng ngay

    const subTaskId = $(this).data('subtask-id');
    const popup = $(`#listAssignPopup-${subTaskId}`);

    // Ẩn tất cả popup khác
    $('.list-assign-popup-container').not(popup).addClass('d-none');

    // Toggle popup hiện tại
    popup.toggleClass('d-none');
});

// Ẩn popup phân công thành viên khi bấm ra ngoài
$(document).on('click', function (e) {
    if (!$(e.target).closest('.list-assign-popup-container').length &&
        !$(e.target).closest('.list-assign-toggle').length) {
        $('.list-assign-popup-container').addClass('d-none');
    }
});

function initDueDateFlatpickr() {
    $(".flatpickr-input").each(function () {
        flatpickr(this, {
            inline: true,
            defaultDate: $(this).val() || null,
            onChange: function (selectedDates, dateStr) {
                $(this._input).data("selected-date", dateStr);
            }
        });
    });
}

