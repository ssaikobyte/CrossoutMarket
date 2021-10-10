
var blockyTypes = {};
blockyTypes['match'] = {
    color: '#dfe',
    size: 16,
    scale: 4
};
blockyTypes['group'] = {
    color: '#f33',
    size: 10,
    scale: 2
};
blockyTypes['profile'] = {
    color: '#bfff00',
    size: 16,
    scale: 4
};


function placeBlockies() {
    $('.blocky').each(function (i, e) {
        var seed = $(e).data('seed');
        var type = $(e).data('blockytype');
        var settings = blockyTypes[type];

        var icon = blockies.create({ // All options are optional
            seed: seed.toString(), // seed used to generate icon data, default: random
            color: settings.color, // to manually specify the icon color, default: random
            bgcolor: '#aaa', // choose a different background color, default: random
            size: settings.size, // width/height of the icon in blocks, default: 8
            scale: settings.scale, // width/height of each block in pixels, default: 4
            spotcolor: '#000' // each pixel has a 13% chance of being of a third color,
            // default: random. Set to -1 to disable it. These "spots" create structures
            // that look like eyes, mouths and noses.
        });

        $(e).html(icon);
    });
};