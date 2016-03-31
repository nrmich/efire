#!/usr/bin/env node
var WebSocketServer = require('websocket').server;
var http = require('http');
var Random = require('./node_modules/simjs/dist/random-node-0.25.js');
//var sim = require('simjs');

// Spawning python child
var spawn = require("child_process").spawn;
const emitter = new EventEmitter()
emitter.setMaxListeners(100)
// Filesystem for writing to file
var fs = require('fs');

// Random for creating Gaussian distributions
var random = new Random(5);

var server = http.createServer(function(request, response) {
    console.log((new Date()) + ' Received request for ' + request.url);
    response.writeHead(404);
    response.end();
});

//server.listen(8080, function() {
//    console.log((new Date()) + ' Server is listening on port 8080');
//});

server.listen(1025, function() {
    console.log((new Date()) + ' Server is listening on port 1025');
});
 
wsServer = new WebSocketServer({
    httpServer: server,
    // You should not use autoAcceptConnections for production 
    // applications, as it defeats all standard cross-origin protection 
    // facilities built into the protocol and the browser.  You should 
    // *always* verify the connection's origin and decide whether or not 
    // to accept it. 
    autoAcceptConnections: false
});
 
function originIsAllowed(origin) {
  // put logic here to detect whether the specified origin is allowed. 
  return true;
}
 
wsServer.on('request', function(request) {
    if (!originIsAllowed(request.origin)) {
      // Make sure we only accept requests from an allowed origin 
      request.reject();
      console.log((new Date()) + ' Connection from origin ' + request.origin + ' rejected.');
      return;
    }
    
    var connection = request.accept('echo-protocol', request.origin);
    console.log((new Date()) + ' Connection accepted.');
    connection.on('message', function(message) {
        if (message.type === 'utf8') {
            console.log('Received Message: ' + message.utf8Data);
            connection.sendUTF(message.utf8Data);
            // One datapoint per second
            // setInterval(function(){sendMirocDataPoint(connection);},1000);
            
            // 100 datapoints per second
            setInterval(function(){sendMirocDataPoint(connection);},10);
            
            // 200 datapoints per second
            //setInterval(function(){sendMirocDataPoint(connection);},5);
            
            // 500 datapoints per second
            //setInterval(function(){sendMirocDataPoint(connection);},2);
            
            // 1000 datapoints per second
            // setInterval(function(){sendMirocDataPoint(connection);},1);
        }
        else if (message.type === 'binary') {
            console.log('Received Binary Message of ' + message.binaryData.length + ' bytes');
            connection.sendBytes(message.binaryData);
        }
    });
    connection.on('close', function(reasonCode, description) {
        console.log((new Date()) + ' Peer ' + connection.remoteAddress + ' disconnected.');
    });
});


// n counts number of strikes sent
var n = 0;
var process = spawn('python', ["dummy_data.py"]);

function sendMirocDataPoint(connection){
	/* child process */
	process.stdout.on('data', function(data){
			console.log(data.toString('utf8'))
			});
	/* Old JS Dummy Data ----------------------------------------------
	var center = Math.floor(Math.random()*4) + 1;
	var bin = Math.floor(random.normal(center*64, 10));
	var dataPoint = {
			strike: n, 
			time: Date().toString(), 
			diode: Math.floor(Math.random() * 13) + 1 , 
			energy: bin * 4, 
			bin: bin 
			}
	connection.send(JSON.stringify(dataPoint));
        console.log("sent datapoint " + JSON.stringify(dataPoint));
	------------------------------------------------------------------- */

	
	n++;
	//console.log(dataPoint);
}
