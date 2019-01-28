function createChartParams(data) {
    var renderSpeed = data.map(function (item) {
        return item.Speed;
    });

    var renderMap = data.map(function (item) {
        return item.MapLink; 
    });

    var params = {
        type: 'horizontalBar',
        data: {
            labels: renderMap,
            datasets: [{
                label: 'Load Time ms',
                data: renderSpeed,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    };
    return params;
}