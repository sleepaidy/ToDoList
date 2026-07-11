(function () {
    const notificationsRoot = document.querySelector(".notifications");
    if (!notificationsRoot || typeof signalR === "undefined") {
        return;
    }

    function getL10n() {
        try {
            const el = document.getElementById("app-localization");
            return el ? JSON.parse(el.textContent) : {};
        } catch {
            return {};
        }
    }

    const l10n = getL10n();

    function buildMessage(taskName, deadlineFormatted, reminderType) {
        if (reminderType === "1h") {
            return (l10n.notificationDeadline1h || "Task \"{0}\" expires in 1 hour ({1})")
                .replace("{0}", taskName)
                .replace("{1}", deadlineFormatted);
        }
        return (l10n.notificationDeadline24h || "Task \"{0}\" expires in 24 hours ({1})")
            .replace("{0}", taskName)
            .replace("{1}", deadlineFormatted);
    }

    function showNotification(text) {
        const div = document.createElement("div");
        div.className = "notification";
        div.textContent = text;
        div.addEventListener("click", () => div.remove());
        notificationsRoot.appendChild(div);

        setTimeout(() => div.remove(), 8000);
    }

    const hub = new signalR.HubConnectionBuilder()
        .withUrl("/my-hub/todo")
        .withAutomaticReconnect()
        .build();

    hub.on("DeadlineApproaching", function (taskId, taskName, deadlineFormatted, reminderType) {
        showNotification(buildMessage(taskName, deadlineFormatted, reminderType));
    });

    hub.start().catch(function (err) {
        console.error("SignalR connection error:", err);
    });
})();