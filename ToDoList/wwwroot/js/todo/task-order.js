$(function () {
    function getTaskOrderErrorMessage() {
        try {
            const el = document.getElementById('app-localization');
            if (!el) {
                return 'Could not reorder the task';
            }
            const l10n = JSON.parse(el.textContent);
            return l10n.taskOrderError || 'Could not reorder the task';
        } catch {
            return 'Could not reorder the task';
        }
    }

    $('.task-list--sortable').each(function () {
        updateOrderButtons($(this));
    });

    $('.task-list--sortable').on('click', '.task-card__order-btn:not(:disabled)', function () {
        const $btn = $(this);
        const $item = $btn.closest('[data-task-id]');
        const taskId = $item.data('taskId');
        const isUp = $btn.hasClass('task-card__order-btn--up');
        const url = isUp
            ? `/api/ToDoList/MoveUp?id=${taskId}`
            : `/api/ToDoList/MoveDown?id=${taskId}`;

        $btn.prop('disabled', true);

        $.post(url)
            .done(function () {
                moveItemInDom($item, isUp);
                updateOrderButtons($item.closest('.task-list--sortable'));
            })
            .fail(function () {
                alert(getTaskOrderErrorMessage());
            })
            .always(function () {
                updateOrderButtons($item.closest('.task-list--sortable'));
            });
    });

    function moveItemInDom($item, isUp) {
        if (isUp) {
            const $prev = $item.prev('[data-task-id]');
            if ($prev.length) {
                $item.insertBefore($prev);
            }
        } else {
            const $next = $item.next('[data-task-id]');
            if ($next.length) {
                $item.insertAfter($next);
            }
        }
    }

    function updateOrderButtons($list) {
        const $items = $list.children('[data-task-id]');

        $items.each(function (index) {
            const $order = $(this).find('.task-card__order');
            $order.find('.task-card__order-btn--up').prop('disabled', index === 0);
            $order.find('.task-card__order-btn--down').prop('disabled', index === $items.length - 1);
        });
    }
});
