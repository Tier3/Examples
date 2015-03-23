/**
 * Modified by Bryan on 3/22/2015.
 */

var socket = io.connect(window.location.hostname);
socket.on('connect', function () {
    $('#status').append('<b>Connected!</b>');
});

socket.on('webhookacctmessage', function(data, signatureHeader){
    var popoverData = JSON.stringify(data, null, 4).replace(/"/g, '&quot;').replace(/{/g, '&#123;').replace(/}/g, '&#125;');
    //var truncatedSig = signatureHeader.substring(0,15) + " ...";
    popoverData = popoverData + "<br/><br/><b>Signature:</b> " + signatureHeader;

    $('#acctchangefeed').append('<div rel="popover" class="webhookmessage" data-content="'+ popoverData +'" title="Account Message">' + data.businessName + '<br>(' + data.alias + ')</div>');
    $("[rel=popover]").popover({placement:'right', html:true});
});

socket.on('webhookalertmessage', function(data, signatureHeader){
    var popoverData = JSON.stringify(data, null, 4).replace(/"/g, '&quot;').replace(/{/g, '&#123;').replace(/}/g, '&#125;');
    popoverData = popoverData + "<br/><br/><b>Signature:</b> " + signatureHeader;

    $('#alertchangefeed').append('<div rel="popover" class="webhookmessage" data-content="'+ popoverData +'" title="Account Message">' + data.businessName + '<br>(' + data.alias + ')</div>');
    $("[rel=popover]").popover({placement:'right', html:true});
});

socket.on('webhookusermessage', function(data, signatureHeader){

    var popoverData = JSON.stringify(data, null, 4).replace(/"/g, '&quot;').replace(/{/g, '&#123;').replace(/}/g, '&#125;');
    popoverData = popoverData + "<br/><br/><b>Signature:</b> " + signatureHeader;

    $('#userchangefeed').append('<div rel="popover" class="webhookmessage" data-content="'+ popoverData +'" title="User Message">' + data.userName +' </div>');
    $("[rel=popover]").popover({placement:'right', html:true});
});

socket.on('webhookservermessage', function(data, signatureHeader){

    var popoverData = JSON.stringify(data, null, 4).replace(/"/g, '&quot;').replace(/{/g, '&#123;').replace(/}/g, '&#125;');
    popoverData = popoverData + "<br/><br/><b>Signature:</b> " + signatureHeader;

    $('#serverchangefeed').append('<div rel="popover" class="webhookmessage" data-content="'+ popoverData +'" title="Server Message">' + data.name +' </div>');
    $("[rel=popover]").popover({placement:'left', html:true});
});
