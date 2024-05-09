
function Validator(formSelector) {
    var _this = this;
    var formRules = {};

    function getParent(element, selector) {
        while (element.parentElement) {
            if (element.parentElement.matches(selector)) {
                return element.parentElement;
            }
            element = element.parentElement;
        }
    }

    var validatorRules = {
        required: function (value) {
            return value ? undefined : 'Vui lòng nhập trường này';
        },
        email: function (value) {
            var regex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            return regex.test(value) ? undefined : 'Vui lòng nhập email';
        },
        min: function (min) {
            return function (value) {
                return value.length >= min ? undefined : `Vui lòng nhập tối thiểu ${min} ký tự`;
            };
        },
        max: function (max) {
            return function (value) {
                return value.length <= max ? undefined : `Vui lòng nhập tối đa ${max} ký tự`;
            };
        },
        checkbox: function (value) {
            return value ? undefined : "Vui lòng chọn Đồng Ý với các Chính Sách Hoạt Động của KFC Việt Nam.";
        },
        numeric: function (value) {
            var regex = /^[0-9]+$/;
            return regex.test(value) ? undefined : 'Vui lòng nhập số điện thoại';
        },
    };

    var formElement = document.querySelector(formSelector);

    if (formElement) {
        var inputs = formElement.querySelectorAll('[name][rules]');
        for (var input of inputs) {
            var rules = input.getAttribute('rules').split('|');
            for (var rule of rules) {
                var ruleInfo;
                var isRuleHasValue = rule.includes(':');
                if (isRuleHasValue) {
                    ruleInfo = rule.split(':');
                    rule = ruleInfo[0];
                }

                var ruleFunc = validatorRules[rule];
                if (isRuleHasValue) {
                    ruleFunc = ruleFunc(ruleInfo[1]);
                }

                if (Array.isArray(formRules[input.name])) {
                    formRules[input.name].push(ruleFunc);
                } else {
                    formRules[input.name] = [ruleFunc];
                }
                if (rule === 'numeric') {
                    formRules[input.name].push(validatorRules.numeric);
                }
            }

            input.onblur = handleValidate;
            input.oninput = handleClear;
        }


        function handleValidate(e) {
            var rules = formRules[e.target.name];
            var errorMessage;

            for (var rule of rules) {
                errorMessage = rule(e.target.value);
                if (e.target.type === 'checkbox' && !errorMessage) {
                    errorMessage = rule(e.target.checked);
                }

                if (errorMessage) break;
            }

            if (errorMessage) {
                var formGroup = getParent(e.target, '.Body_form');
                if (formGroup) {
                    formGroup.classList.add('invalid');
                    var formMessage = formGroup.querySelector('.form-message');
                    if (formMessage) {
                        formMessage.innerText = errorMessage;
                    }
                }
            } else {
                var formGroup = getParent(e.target, '.Body_form');
                if (formGroup) {
                    formGroup.classList.remove('invalid');
                    var formMessage = formGroup.querySelector('.form-message');
                    if (formMessage) {
                        formMessage.innerText = '';
                    }
                }
            }

            return !errorMessage;
        }

        function handleClear(e) {
            var formGroup = getParent(e.target, '.Body_form');
            if (formGroup.classList.contains('invalid')) {
                formGroup.classList.remove('invalid');
                var formMessage = formGroup.querySelector('.form-message');
                if (formMessage) {
                    formMessage.innerText = '';
                }
            }
        }
    }

    formElement.onsubmit = function (e) {
        e.preventDefault();
        var inputs = formElement.querySelectorAll('[name][rules]');
        var isValid = true;

        for (var input of inputs) {
            if (!handleValidate({ target: input })) {
                isValid = false;
            }
        }

        if (isValid) {
            if (typeof _this.onSubmit === 'function') {
                var enableInputs = formElement.querySelectorAll('[name]');
                var formValues = Array.from(enableInputs).reduce(function (values, input) {
                    switch (input.type) {
                        case 'radio':
                            values[input.name] = formElement.querySelector('input[name="' + input.name + '"]:checked').value;
                            break;
                        case 'checkbox':
                            if (!Array.isArray(values[input.name])) {
                                values[input.name] = [];
                            }
                            if (input.matches(":checked")) {
                                values[input.name].push(input.value);
                            }
                            break;
                        case 'file':
                            values[input.name] = input.files;
                            break;
                        default:
                            values[input.name] = input.value;
                            break;
                    }
                    return values;
                }, {});
                _this.onSubmit(formValues);
            } else {
                formElement.submit();
            }
        }
    };
}

//Part Show-hide Password
const iconPassword = $('.icon-password');
iconPassword.click(function () {
    var inputPassword = $('.input-password');
    var currentType = inputPassword.attr('type');

    if (currentType === "password") {
        iconPassword.html('<ion-icon name="eye-off"></ion-icon>')
        inputPassword.attr('type', 'text');
    } else {
        iconPassword.html('<ion-icon name="eye"></ion-icon>')
        inputPassword.attr('type', 'password');
    }
});

//Part check value input
const input = $('.form_input');
input.on('input', function () {
    if ($(this).val().trim() !== '') {
        $(this).addClass('has-value');
    } else {
        $(this).removeClass('has-value');
    }
});

//Part View MyAccount
//Part Active link
$('.list-group').on('click', '.list-group-item', function () {
    $('.list-group .list-group-item.active').removeClass('active');
    $(this).addClass('active');
});

//Show/hide link items
$('.list-group-item').click(function () {
    $('.collapse').collapse('hide');
});

