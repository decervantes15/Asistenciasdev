async function AjaxPost(url, data, successCallBack) {
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(data),
        success: successCallBack,
        error: function (e) {
            console.log(e);
        },
        cache: false,
        async: false
    });
}
async function AjaxPostNet(url, data, successCallBack) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: successCallBack,
        error: function (e) {
            console.log(e);
        },
        cache: false,
        async: false
    });
}