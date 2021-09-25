
var matchData = [];

var matchId = $('.team-table').data('matchid');

$.ajax({
    url: '/data/match/' + matchId,
    dataType: 'json',
    success: function (json) {
        matchData = json;
        $('td.details-control > .loading').addClass('d-none');
        $('td.details-control > svg.plus').removeClass('d-none');
    }
});

var tables = [];
$('.team-table').each(function (i, e) {
    var id = $(e).data('tableid');
    var roundId = $(e).data('roundid');
    var table = $(e).DataTable({
        dom: '<tr>',
        order: [[8, 'desc']],
        searching: true,
        lengthChange: false,
        paging: false,
        info: false,
        autoWidth: false,
        columnDefs: [
            { width: '15%', targets: [1, 3, 7, 8] },
            { width: '12%', targets: [4, 5, 6, 2] },
            { orderable: false, width: '4%', targets: [0] },
            { visible: false, targets: [9] },
        ]
    });

    tables.push({ id: id, roundId: roundId, table: table });
});

$('.team-table tbody').on('click', 'td.details-control', function () {
    var teamTable = $(this).closest('.team-table');
    var id = $(teamTable).data('tableid');
    var table = tables.find(x => x.id === id);
    var tr = $(this).closest('tr');
    var row = table.table.row(tr);

    if (row.child.isShown()) {
        row.child.hide();
        tr.removeClass('shown');
        $(this).children('td.details-control > svg.minus').addClass('d-none');
        $(this).children('td.details-control > svg.plus').removeClass('d-none');
    }
    else {
        row.child(format(table, row.data())).show();
        tr.addClass('shown');
        $(this).children('td.details-control > svg.plus').addClass('d-none');
        $(this).children('td.details-control > svg.minus').removeClass('d-none');
    }
});

function format(table, rowData) {
    var roundId = table.roundId;

    var div = $('<div/>');

    var html = '<div class="d-flex flex-row-reverse justify-content-between"><table class="table table-borderless" style="width:50%">' +
        '<thead>' +
        '<tr>' +
        '<th>Weapon</th>' +
        '<th>Damage</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>';

    var damages = [];
    if (roundId >= 0)
        damages = matchData.damageData.filter(x => x.userId === parseInt(rowData[9]) && x.roundId === roundId);
    else {
        damages = matchData.damageData.filter(x => x.userId === parseInt(rowData[9]));
        var combinedDamages = [];
        damages.forEach(function (e, i) {
            var combinedIndex = combinedDamages.findIndex(x => x.itemId === e.itemId);
            if (combinedIndex === -1) {
                combinedDamages.push(JSON.parse(JSON.stringify(e)));
            }
            else {
                combinedDamages[combinedIndex].damage += e.damage;
            }
        });
        damages = combinedDamages;
    }

    damages.sort((a, b) => (a.damage < b.damage) ? 1 : -1);

    damages.forEach(function (e, i) {
        var image = ''
        if (e.imageExists)
            image = '<img class="mr-1" width="32"height="32" src="/img/items/' + e.itemId + '.png"></img>';
        var name = e.weaponDisplayName
        if (e.itemId !== 0)
            name = '<a href="/item/' + e.itemId + '">' + e.weaponDisplayName + '</a>';
        html += '<tr><td>' + image + name + '</td><td>' + Math.round(e.damage * 10) / 10 + '</td></tr>';
    });
    html += '</tbody>';


    var medals = [];
    if (roundId >= 0)
        medals = matchData.medalData.filter(x => x.userId === parseInt(rowData[9]) && x.roundId === roundId);
    else {
        medals = matchData.medalData.filter(x => x.userId === parseInt(rowData[9]));
        var combinedMedals = [];
        medals.forEach(function (e, i) {
            var combinedIndex = combinedMedals.findIndex(x => x.medal === e.medal);
            if (combinedIndex === -1) {
                combinedMedals.push(JSON.parse(JSON.stringify(e)));
            }
            else {
                combinedMedals[combinedIndex].amount += e.amount;
            }
        });
        medals = combinedMedals;
    }

    html += '<div class="d-flex flex-row flex-wrap" style="width:50%">';

    medals.sort((a, b) => (a.medal > b.medal) ? 1 : -1);

    medals.forEach(function (e, i) {
        html += '<div class="overlay-container"><img class="" width="64" height="64" src="/img/medals/' + e.medal + '.png"><div class="img-overlay-medal-amount">' + e.amount + '</div></div>';
    });

    html += '</div></div>';

    div.html(html);

    return div;
}

$('.dropdown-item').click(function (e) {
    var tableId = $(this).parent().data('tableid');
    var wrapperId = '#visualizationWrapper' + tableId;
    if ($(this).hasClass('active')) {
        $(this).removeClass('active');
        $(wrapperId).addClass('d-none');
    }
    else {
        $(this).parent().children().removeClass('active');
        $(this).addClass('active');
        var colIndex = $(this).data('columnindex');
        var label = $(this).text();
        createPieChart(tableId, colIndex, label);
        $(wrapperId).removeClass('d-none');
    }
});

var charts = [];
function createPieChart(tableId, columnIndex, label) {
    var wrapperId = 'piechartWrapper' + tableId;
    var table = tables.find(x => x.id === tableId);
    var data = [];
    table.table.cells().every(function (cellRowIndex, cellColumnIndex, tableLoopCounter, cellLoopCounter) {
        if (cellColumnIndex === columnIndex) {
            var node = this.node();
            var value = parseFloat($(node).data('order'));
            var name = table.table.cell({ row: cellRowIndex, column: 1 }).data();
            var dataPoint = { name: name, y: value };
            data.push(dataPoint);
        }
    });

    var indexOfChart = charts.findIndex(x => x.tableId === tableId);
    if (indexOfChart !== -1)
        charts[indexOfChart].chart.destroy();

    var chart = Highcharts.chart(wrapperId, {
        chart: {
            backgroundColor: null,
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        pane: {
            background: {
                backgroundColor: null
            }
        },
        title: {
            text: label
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                borderColor: null,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        textOutline: 'none'
                    }
                }
            },
            series: {
                dataSorting: {
                    enabled: true
                }
            }
        },
        series: [{
            name: label,
            colorByPoint: true,
            data: data
        }],
        exporting: {
            enabled: false
        }
    });

    var indexOfChart = charts.findIndex(x => x.tableId === tableId);
    if (indexOfChart === -1)
        charts.push({ tableId: tableId, chart: chart });
    else {
        charts[indexOfChart].chart = chart;
    }
}