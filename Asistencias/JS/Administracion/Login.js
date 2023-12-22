$(document).on('click', '#enter', function () {

    var aviso = $('#aviso');
    aviso.removeClass('text-success');
    aviso.addClass('text-danger');
    aviso.html('');

    var username = $('#user').val();
    var password = $('#password').val();

    if (username.length == 0) {
        aviso.html('<br /> Debe de ingresar el usuario');
    }
    else if (password.length == 0) {
        aviso.html('<br /> Debe de ingresar la contraseña');
    }
    else {
        AjaxPost('/api/v1/admin/login',
            { username, password },
            function (respuesta) {
                aviso.html('<br />' + respuesta.Mensaje);
                if (respuesta.Estatus == 1) {
                    window.location.href = "/Administracion/Asistencias";
                }
            }
        );
    }
});
