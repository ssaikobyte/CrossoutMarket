
//synergyWrapper

//const { each } = require("jquery");

// VARS
var synergyData = {
    data: {},
    loaded: false
};

var synergies = {
    tree: {
        topToBottom: [],
        visible: []
    },
};

// INIT
$(document).ready(function () {

});

function onSynergyDataLoaded() {
    synergyList = synergyData.data.synergies;
    itemList = synergyData.data.synergyitems;
    var uniqueId = 0;
    synergyList.forEach(function (s, i) {
        mapSynergyItems(null, { synergy: s}, 0, uniqueId);
        uniqueId++;
        itemList.forEach(function (e, i) {
            if (e.synergy.synergyType == s.synergyType) {
                mapSynergyItems(s, e, 1, uniqueId);
                uniqueId++;
            }
        });
    });
    drawSynergy();
/*
    var wrapper = $('#synergyWrapper').append('<div>');
    //wrapper.children().remove();
    var treeWrapper = $('<div class="col-4">' + 'TEST' + '</div>').appendTo(wrapper);
*/
}

function mapSynergyItems(rootDisplayItem, item, currentDepth, uniqueId) {
    var displayItem = {
        uniqueId: uniqueId,
        synergyType: item.synergy.synergyType,
        itemNumber: item.synergy.itemNumber,
        name: item.itemLoc === undefined ? item.synergy.synergyType : item.itemLoc.LocalizedName,
        show: true,
        expanded: true,
        depth: currentDepth,
        hasSynergies: false,
        rootDisplayItem: rootDisplayItem,
    }

    if (displayItem.depth == 0) {
        displayItem.name = item.synergy.synergyType;
        displayItem.hasSynergies = true;
        expanded = false;
    }
    synergies.tree.topToBottom.push(displayItem);
}

function drawSynergy() {
    var wrapper = $('#synergyWrapper').append('<div>');
    wrapper.children().remove();
    var treeWrapper = $('<div class="col-4"></div>').appendTo(wrapper);
    drawSynergyTreeHeader(treeWrapper);
    synergies.tree.topToBottom.forEach(function (e, i) {
        if (e.show) {
            drawSynergyTreeEntry(e, treeWrapper);
            synergies.tree.visible.push(e);
        }
    });
    bindEvents();
    $('[data-toggle="tooltip"]').tooltip();
    bindSynergyEvents();
}

function drawSynergyTreeHeader(wrapper) {
    var html = '<div class="d-flex flex-row justify-content-between my-1 mx-1">' +
        '<div class="d-flex flex-row justify-content-between w-50">' +
        '<div class="font-weight-bold">' +
        '' +
        '</div>' +
        '</div>'
    $(wrapper).append(html);
}

function drawSynergyTreeEntry(displayItem, wrapper) {
    var depthSpacer = '';
    for (var i = 0; i < displayItem.depth; i++) {
        depthSpacer += '<div style="width: 24px;"></div>';
    }
    var expandButton = '<button class="btn btn-sm btn-outline-secondary synergy-expand-btn text-monospace ' + (displayItem.hasSynergies ? '' : 'invisible') + '" data-uniqueid="' + displayItem.uniqueId + '">' + (displayItem.expanded ? '-' : '+') + '</button>';

    var html = '<div class="d-flex flex-row justify-content-between my-1 mx-1"">' +

        '<div class="d-flex flex-row w-50">' +
        depthSpacer +
        (displayItem.rootDisplayItem == null ? expandButton : '') +
        '<a href="/item/' + displayItem.itemNumber + '">' +
        '<div class="d-flex flex-row">' +
        (displayItem.depth > 0 ? '<img class="ml-1 item-image-med" src="' +
        '/img/items/' + displayItem.itemNumber + '.png' +
        '"/ >' : '') +
        '<div class="ml-1">' +
        displayItem.name +
        '</div>' +
        '</div>' +
        '</a>';
    $(wrapper).append(html);
}

// MANIPULATE
function setSynergyCollapse(uniqueId, collapse) {
    var inTarget = false;
    var targetDepth = 0;
    synergies.tree.topToBottom.forEach(function (e, i) {
        if (inTarget && e.depth > targetDepth) {
            if (e.depth > targetDepth + 1)
                e.show = false;
            else
                e.show = !collapse;
            if (e.hasSynergies)
                e.expanded = false;
        } else {
            inTarget = false;
        }

        if (e.uniqueId === uniqueId) {
            inTarget = true;
            targetDepth = e.depth;
            e.expanded = !collapse;
        }
    });
}

// UPDATE
function expandSynergy(uniqueId, expand) {
    setSynergyCollapse(uniqueId, !expand);
    drawSynergy();
}

// EVENT HANDLERS
function bindSynergyEvents() {
    $('.synergy-expand-btn').click(function () {
        var uniqueId = parseInt($(this).attr('data-uniqueid'));
        if (getSynergyExpandedStatus(uniqueId))
            expandSynergy(uniqueId, false);
        else {
            expandSynergy(uniqueId, true);
        }
    });
}

// HELPERS
function getSynergyExpandedStatus(uniqueId) {
    return synergies.tree.topToBottom.find(x => x.uniqueId === uniqueId).expanded;
}

