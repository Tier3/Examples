
/**
 * Module dependencies.
 */

var express = require('express');
//var routes = require('./routes');
var http = require('http');
var path = require('path');
var fs = require('fs');
var rsa = require('node-rsa');

var app = express();
//create server
var server = http.createServer(app);
//create socket.io reference
var io = require('socket.io').listen(server);
var port = process.env.PORT || 3000;

// all environments
app.set('views', __dirname + '/views');
app.set('view engine', 'jade');
app.use(express.favicon());
app.use(express.logger('dev'));
app.use(express.bodyParser());
app.use(express.methodOverride());
app.use(app.router);
app.use(express.static(path.join(__dirname, 'public')));

// development only
if ('development' == app.get('env')) {
  app.use(express.errorHandler());
}

app.get('/', function(req, res){
    res.render('index', { title: 'Webhook Listener  ' });
})

app.post('/webhook/account', function(req, res){

    var signatureHeader = req.get('Tier3-RsaSha1');
    BroadcastAccountWebhook(req.body, signatureHeader);
    res.send("ok");
})

app.post('/webhook/alert', function(req, res){

    var signatureHeader = req.get('Tier3-RsaSha1');
    BroadcastAlertWebhook(req.body, signatureHeader);
    res.send("ok");
})

app.post('/webhook/user', function(req, res){

    var signatureHeader = req.get('Tier3-RsaSha1');
    BroadcastUserWebhook(req.body, signatureHeader);
    res.send("ok");
})

app.post('/webhook/server', function(req, res){

    var signatureHeader = req.get('Tier3-RsaSha1');
    BroadcastServerWebhook(req.body, signatureHeader);
    res.send("ok");
})

//socket.io configuration
io.sockets.on('connection', function (socket) {
    console.log('client connected to socket.io');
});

function VerifySignature(data, signatureHeader) {

  var publicKey = fs.readFileSync(path.resolve(__dirname, 'public.pem')).toString();

  var key = new rsa(publicKey, 'pkcs8-public-pem', {"signingScheme":"sha1"});

  return key.verify(data, signatureHeader, 'utf8', 'base64');

}

function BroadcastAccountWebhook(data, signatureHeader){

    verify = VerifySignature(data, signatureHeader);

    io.sockets.emit('webhookacctmessage', data, signatureHeader, verify);

    console.log(data);
}

function BroadcastAlertWebhook(data, signatureHeader){

    verify = VerifySignature(data, signatureHeader);

    io.sockets.emit('webhookalertmessage', data, signatureHeader, verify);

    console.log(data);
}


function BroadcastUserWebhook(data, signatureHeader){

    verify = VerifySignature(data, signatureHeader);

    io.sockets.emit('webhookusermessage', data, signatureHeader, verify);

    console.log(data);
}

function BroadcastServerWebhook(data, signatureHeader){

    verify = VerifySignature(data, signatureHeader);

    io.sockets.emit('webhookservermessage', data, signatureHeader, verify);

    console.log(data);
}

//start up server
server.listen(port);
console.log('Realtime app running on port ' + port);
