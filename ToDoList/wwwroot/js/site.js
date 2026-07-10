(function () {
    function getAppLocalization() {
        try {
            const el = document.getElementById("app-localization");
            return el ? JSON.parse(el.textContent) : {};
        } catch {
            return {};
        }
    }

    const l10n = getAppLocalization();
    const locale = l10n.locale || "ru-RU";
    const monthNames = l10n.months || [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];

    function pad(value) {
        return String(value).padStart(2, "0");
    }

    function toDateKey(date) {
        return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
    }

    function formatTemplate(template, value) {
        return String(template).replace("{0}", value);
    }

    function updateClock() {
        const clock = document.getElementById("dashboard-clock");
        const dateLabel = document.getElementById("dashboard-date");
        if (!clock || !dateLabel) {
            return;
        }

        const now = new Date();
        clock.textContent = `${pad(now.getHours())}:${pad(now.getMinutes())}`;
        clock.setAttribute("datetime", now.toISOString());

        const dateText = now.toLocaleDateString(locale, {
            weekday: "long",
            day: "numeric",
            month: "long"
        });
        dateLabel.textContent = dateText.charAt(0).toUpperCase() + dateText.slice(1);
    }

    function initCalendar() {
        const calendarRoot = document.getElementById("task-calendar");
        const grid = document.getElementById("calendar-grid");
        const monthLabel = document.getElementById("calendar-month-label");
        const prevBtn = document.getElementById("calendar-prev");
        const nextBtn = document.getElementById("calendar-next");
        const dayPanel = document.getElementById("calendar-day-panel");
        const dayTitle = document.getElementById("calendar-day-title");
        const dayList = document.getElementById("calendar-day-list");
        const dueDateInput = document.getElementById("task-due-date");

        if (!calendarRoot || !grid || !monthLabel) {
            return;
        }

        let tasksByDate = {};
        try {
            const dataEl = document.getElementById("calendar-tasks-data");
            tasksByDate = JSON.parse(dataEl?.textContent || "{}");
        } catch {
            tasksByDate = {};
        }

        let viewDate = new Date();
        viewDate.setDate(1);
        let selectedDate = toDateKey(new Date());

        function getTopPriorityClass(tasks) {
            if (!tasks || tasks.length === 0) {
                return "";
            }
            if (tasks.some((task) => task.priorityClass === "high")) {
                return "high";
            }
            if (tasks.some((task) => task.priorityClass === "medium")) {
                return "medium";
            }
            return "low";
        }

        function showDayPanel(dateKey) {
            if (!dayPanel || !dayTitle || !dayList) {
                return;
            }

            const tasks = tasksByDate[dateKey] || [];
            if (tasks.length === 0) {
                dayPanel.hidden = true;
                return;
            }

            const parts = dateKey.split("-").map(Number);
            const date = new Date(parts[0], parts[1] - 1, parts[2]);
            const label = date.toLocaleDateString(locale, {
                day: "numeric",
                month: "long"
            });

            dayTitle.textContent = formatTemplate(l10n.tasksOn || "Tasks on {0}", label);
            dayList.innerHTML = tasks.map((task) => {
                const statusLabel = task.statusClass === "done"
                    ? (l10n.statusDone || "done")
                    : task.statusClass === "failed"
                        ? (l10n.statusFailed || "overdue")
                        : (l10n.statusActive || "active");
                return `<li class="calendar-day-panel__item calendar-day-panel__item--${task.priorityClass}">
                    <span class="calendar-day-panel__task">${escapeHtml(task.name)}</span>
                    <span class="calendar-day-panel__status">${statusLabel}</span>
                </li>`;
            }).join("");

            dayPanel.hidden = false;
        }

        function escapeHtml(text) {
            return String(text)
                .replace(/&/g, "&amp;")
                .replace(/</g, "&lt;")
                .replace(/>/g, "&gt;")
                .replace(/"/g, "&quot;");
        }

        function renderCalendar() {
            const year = viewDate.getFullYear();
            const month = viewDate.getMonth();
            monthLabel.textContent = `${monthNames[month]} ${year}`;

            const firstWeekday = (new Date(year, month, 1).getDay() + 6) % 7;
            const daysInMonth = new Date(year, month + 1, 0).getDate();
            const todayKey = toDateKey(new Date());

            grid.innerHTML = "";

            for (let i = 0; i < firstWeekday; i++) {
                const empty = document.createElement("span");
                empty.className = "widget-calendar__cell widget-calendar__cell--empty";
                empty.setAttribute("aria-hidden", "true");
                grid.appendChild(empty);
            }

            for (let day = 1; day <= daysInMonth; day++) {
                const date = new Date(year, month, day);
                const dateKey = toDateKey(date);
                const tasks = tasksByDate[dateKey] || [];
                const button = document.createElement("button");
                button.type = "button";
                button.className = "widget-calendar__cell widget-calendar__day";
                button.dataset.date = dateKey;
                button.textContent = String(day);

                if (dateKey === todayKey) {
                    button.classList.add("widget-calendar__day--today");
                }
                if (dateKey === selectedDate) {
                    button.classList.add("widget-calendar__day--selected");
                }
                if (tasks.length > 0) {
                    button.classList.add("widget-calendar__day--has-tasks");
                    button.classList.add(`widget-calendar__day--${getTopPriorityClass(tasks)}`);
                    button.title = formatTemplate(l10n.tasksCount || "{0} task(s)", tasks.length);
                }

                grid.appendChild(button);
            }
        }

        grid.addEventListener("click", (event) => {
            const button = event.target.closest("[data-date]");
            if (!button) {
                return;
            }

            selectedDate = button.dataset.date;
            if (dueDateInput) {
                dueDateInput.value = selectedDate;
            }
            renderCalendar();
            showDayPanel(selectedDate);
        });

        prevBtn?.addEventListener("click", () => {
            viewDate.setMonth(viewDate.getMonth() - 1);
            renderCalendar();
        });

        nextBtn?.addEventListener("click", () => {
            viewDate.setMonth(viewDate.getMonth() + 1);
            renderCalendar();
        });

        renderCalendar();
        showDayPanel(selectedDate);
    }

    updateClock();
    setInterval(updateClock, 30000);
    initCalendar();
})();
