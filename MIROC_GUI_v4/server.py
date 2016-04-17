import tornado.httpserver
import tornado.websocket
import tornado.ioloop
import tornado.web
import socket
import json
import random
import math
import time
import threading

'''
This is a simple Websocket Echo server that uses the Tornado websocket handler.
Please run `pip install tornado` with python of version 2.7.9 or greater to install tornado.
This program will echo back the reverse of whatever it recieves.
Messages are output to the terminal for debuggin purposes. 
''' 
 
class WSHandler(tornado.websocket.WebSocketHandler):
    n = 0;
    def open(self):
        print 'new connection'
      
    def on_message(self, message):
        print 'message received:  %s' % message
        # Reverse Message and send it back
	#data = json.dumps({"strike": 1, "time": 2, "diode": 7, "energy": 100, "bin": 8});
        
        #Nate: working here - try to move to timer
        #data = generateDataPoint(WSHandler.n)
        #print 'sending back message: %s' % data
        #self.write_message(data)
        #WSHandler.n=WSHandler.n+1
        WSHandler.sendLotsaData(self)
 
    def on_close(self):
        print 'connection closed'
        #Nate: TODO we need to stop the timer here
 
    def check_origin(self, origin):
        return True
    def sendLotsaData(self):
        #Nate: sends a datapoint every .01 seconds; timer so should be nonblocking
        threading.Timer(.01, self.sendLotsaData).start()
        data = generateDataPoint(WSHandler.n)
        print 'sending back message: %s' % data
        self.write_message(data)
        WSHandler.n=WSHandler.n+1
    

application = tornado.web.Application([
    (r'/ws', WSHandler),
])

def generateDataPoint(n):
    center = int(random.random()*4) + 1
    mean = center*50
    stdDev = 10
    randNormal = random.gauss(mean, stdDev)
    #hacky way to construct a json object as a string
    #dataPoint = "{\"strike\":" + str(n) + ", \"time\":\"" + time.asctime() + "\", \"diode\":" + str(int(random.random()*12)) + ", \"energy\":" + str(randNormal*4) + ", \"bin:\"" + str(randNormal) + "}"
    dataPoint = json.dumps({"strike": n, "time": time.asctime(), "diode": int(random.random()*12), "energy": randNormal*4, "bin": int(randNormal)});
    return dataPoint
 
 
if __name__ == "__main__":
    http_server = tornado.httpserver.HTTPServer(application)
    http_server.listen(8888)
    myIP = socket.gethostbyname(socket.gethostname())
    print '*** Websocket Server Started at %s***' % myIP
    tornado.ioloop.IOLoop.instance().start()
 
