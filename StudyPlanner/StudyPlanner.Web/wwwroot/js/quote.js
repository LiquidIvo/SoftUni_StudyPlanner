$(document).ready(function () {
    $.getJSON("/api/quote")
        .done(function (data) {
            $("#quote-container").html(
                '<i class="bi bi-quote me-1 opacity-50"></i>' +
                '"' + data.text + '" ' +
                '<span class="fw-semibold">— ' + data.author + '</span>'
            );
        })
        .fail(function () {
            $("#quote-container").html(
                '<i class="bi bi-quote me-1 opacity-50"></i>' +
                '"The secret of getting ahead is getting started." ' +
                '<span class="fw-semibold">— Mark Twain</span>'
            );
        });
});