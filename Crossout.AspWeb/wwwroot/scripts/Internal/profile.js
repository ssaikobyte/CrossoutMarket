var Uid = window.location.pathname.split("/").pop();

$.ajax({
url: '/data/profile/overview_drilldowns/' + Uid,
dataType: 'json',
success: function (json) {
    build_drilldown('gamemode_overview', 'Game Modes', json["gamemode_preference"]);
    build_drilldown('weapons_overview', 'Weapons', json["weapon_preference"]);
    build_drilldown('movement_overview', 'Movement', json["movement_preference"]);
}
});

function build_drilldown(id, title, drilldown_data) {

    var series_data = populate_series_data(drilldown_data);
    var drilldown_data = populate_drilldown_data(drilldown_data);
    var favorite = series_data.reduce((a, b) => a.y > b.y ? a : b);

    console.log(series_data);
    console.log(drilldown_data);

    Highcharts.chart(id, {
        chart: {
            type: 'pie'
        },
        title: {
            text: title
        },
        credits: {
            enabled: false
        },
        subtitle: {
            text: 'Preference ' + favorite.name + '(' + Math.round(favorite.y) + '%)'
        },
        accessibility: {
            announceNewData: {
                enabled: false
            },
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            series: {
                dataLabels: {
                    enabled: false,
                    format: '{point.name}: {point.y:.1f}%'
                }
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:11px">{point.name}</span><br>',
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}%</b> of total<br/>'
        },

        series: [
            {
                name: title,
                colorByPoint: true,
                data: series_data
            }
        ],
        drilldown: {
            series: drilldown_data
        }
    });
}

function populate_series_data(drilldown_data) {
    var total = 0;
    var data = [];

    for (let i = 0; i < drilldown_data.length; i++) {
        total += drilldown_data[i]["count"];

        if (data.filter(function (e) { return e.name === drilldown_data[i]["type"]; }).length > 0) {
            data[data.findIndex(x => x.name === drilldown_data[i]["type"])].y += drilldown_data[i]["count"];
        }
        else {
            data.push({
                name: drilldown_data[i]["type"],
                y: drilldown_data[i]["count"],
                drilldown: drilldown_data[i]["type"]
            });
        }
    }

    for (let i = 0; i < data.length; i++) {
        data[i].y = (data[i].y / total) * 100;
    }

    return data;
}

function populate_drilldown_data(drilldown_data) {
    var data = [];

    for (let i = 0; i < drilldown_data.length; i++) {
        var found_series = false;

        for (let j = 0; j < data.length; j++) {
            if (data[j].series.name == drilldown_data[i]["type"]) {
                data[j].series.data.push(
                    [drilldown_data[i]["name"], drilldown_data[i]["count"]]
                );
                found_series = true;
                break;
            }
        }

        if (!found_series) {
            var obj = {
                'series': {
                    'name': drilldown_data[i]["type"],
                    'id': drilldown_data[i]["type"],
                    'data': [[drilldown_data[i]["name"], drilldown_data[i]["count"]]]
                }
            }

            data.push(obj);
        }
    }

    return data;
}