
//function refreshTasks() {
//    $.get("/User/LoadTasksAjax", function (html) {
//        $("#taskTableContainer").html(html);
//    });
//}

//$(document).ready(function () {

//    $("#taskTableContainer").on("click", ".mark-complete-btn", function () {
//        const id = $(this).data("id");
//        $.post("/Task/Complete/" + id);
//    });

//    $("#taskTableContainer").on("click", ".delete-btn", function () {
//        const id = $(this).data("id");
//        if (confirm("Delete this task?")) {
//            $.post("/Task/Delete/" + id)
//                .done(() => console.log("Task deleted"))
//                .fail(err => alert("Failed to delete task"));
//        }
//    });


//});
