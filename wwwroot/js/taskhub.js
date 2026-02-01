"use strict";

// Global SignalR connection
window.connection = new signalR.HubConnectionBuilder()
    .withUrl("/taskHub")
    .withAutomaticReconnect()
    .build();

connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error(err));

// TaskDeleted event
connection.on("TaskDeleted", function () {
    // User Dashboard
    if ($("#taskTableContainer").length) {
        $.get("/User/LoadTasksAjax", html => {
            $("#taskTableContainer").html(html);
        });
        return;
    }

    // Admin AllTasks
    if ($("#allTasksContainer").length) {
        $.get("/Admin/LoadAllTasksAjax", html => {
            $("#allTasksContainer").html(html);
        });
    }
});

// TaskCompleted event (only reload User Dashboard)
connection.on("TaskCompleted", function () {
    if ($("#taskTableContainer").length) {
        $.get("/User/LoadTasksAjax", html => {
            $("#taskTableContainer").html(html);
        });
    }
});

// Delete task (delegated)
$(document).on("click", ".delete-btn", function () {
    const id = $(this).data("id");
    if (confirm("Delete this task?")) {
        $.post("/Task/Delete/" + id);
    }
});

// Complete task (delegated)
$(document).on("click", ".mark-complete-btn", function () {
    const id = $(this).data("id");
    $.post("/Task/Complete/" + id);
});
