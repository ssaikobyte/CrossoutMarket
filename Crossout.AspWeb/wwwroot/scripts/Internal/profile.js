var profileOverViewData = [];

var Uid = $('.profile-overview').data('matchid');

$.ajax({
url: '/data/profileoverview/' + Uid,
dataType: 'json',
success: function (json) {
    profileOverViewData = json;
    $('td.details-control > .loading').addClass('d-none');
    $('td.details-control > svg.plus').removeClass('d-none');
}
});