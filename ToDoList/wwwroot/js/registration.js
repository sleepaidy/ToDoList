$(function () {
    const $login = $('#auth-login');
    const $submit = $('.auth-form__submit');
    const $form = $('.auth-form');
    const loginTakenMessage = $form.data('loginTakenMessage') || '';
    const $block = $('.login-check-block__inner');
    const $wait = $block.find('.login-check-icon--wait');
    const $ok = $block.find('.login-check-icon--ok');
    const $deny = $block.find('.login-check-icon--deny');
    const $message = $('#login-check-message');

    let debounceTimer = null;
    let requestId = 0;

    if (!$login.length) {
        return;
    }

    $login.on('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(checkLogin, 400);
    });

    if ($login.val().trim()) {
        checkLogin();
    }

    function resetVisualState() {
        $login.removeClass('free used');
        $wait.attr('hidden', 'hidden');
        $ok.attr('hidden', 'hidden');
        $deny.attr('hidden', 'hidden');
        $message.attr('hidden', 'hidden').text('');
    }

    function setLoading() {
        resetVisualState();
        $wait.removeAttr('hidden');
    }

    function setFree() {
        resetVisualState();
        $login.addClass('free');
        $ok.removeAttr('hidden');
        $submit.prop('disabled', false);
    }

    function setUsed() {
        resetVisualState();
        $login.addClass('used');
        $deny.removeAttr('hidden');
        $message
            .text(loginTakenMessage)
            .removeAttr('hidden');
        $submit.prop('disabled', true);
    }

    function checkLogin() {
        const login = $login.val().trim();

        if (!login) {
            resetVisualState();
            $submit.prop('disabled', false);
            return;
        }

        setLoading();

        const currentRequestId = ++requestId;
        const url = `/api/Auth/IsLoginFree?login=${encodeURIComponent(login)}`;

        $.get(url)
            .done(function (isFree) {
                if (currentRequestId !== requestId) {
                    return;
                }

                if (isFree === true) {
                    setFree();
                } else {
                    setUsed();
                }
            })
            .fail(function () {
                if (currentRequestId !== requestId) {
                    return;
                }
                resetVisualState();
                $submit.prop('disabled', false);
            });
    }
});
