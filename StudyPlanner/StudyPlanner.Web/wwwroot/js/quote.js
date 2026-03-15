$(document).ready(function () {
    const cooldown = 5000; // 5 seconds
    const now = Date.now();

   
    const cachedQuote = localStorage.getItem("quoteData");
    const lastFetched = parseInt(localStorage.getItem("quoteTimestamp")) || 0;

    if (cachedQuote && (now - lastFetched) < cooldown) {
       
        const data = JSON.parse(cachedQuote);
        $("#quote-container").html(
            '<i class="bi bi-quote me-1 opacity-50"></i>' +
            '"' + data.text + '" ' +
            '<span class="fw-semibold">— ' + data.author + '</span>'
        );
    } else {
        
        $.getJSON("/api/quote")
            .done(function (data) {
                $("#quote-container").html(
                    '<i class="bi bi-quote me-1 opacity-50"></i>' +
                    '"' + data.text + '" ' +
                    '<span class="fw-semibold">— ' + data.author + '</span>'
                );
                
                localStorage.setItem("quoteData", JSON.stringify(data));
                localStorage.setItem("quoteTimestamp", Date.now());
            })
            .fail(function () {
                $("#quote-container").html(
                    '<i class="bi bi-quote me-1 opacity-50"></i>' +
                    '"The secret of getting ahead is getting started." ' +
                    '<span class="fw-semibold">— Mark Twain</span>'
                );
            });
    }
});