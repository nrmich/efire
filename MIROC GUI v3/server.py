import tornado.httpserver
import tornado.websocket
import tornado.ioloop
import tornado.web
import socket
import json
import random
import math
import time

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
        data = generateDataPoint(WSHandler.n)
        print 'sending back message: %s' % data
        self.write_message(data)
        WSHandler.n=WSHandler.n+1
 
    def on_close(self):
        print 'connection closed'
 
    def check_origin(self, origin):
        return True
    

application = tornado.web.Application([
    (r'/ws', WSHandler),
])

def generateDataPoint(n):
    center = random.random()*4 + 1
    mean = center*40
    stdDev = 15
    u1 = random.random()
    u2 = random.random()
    print u1
    print u2
    randStdNormal = math.sqrt(math.fabs(-2.0*math.log(u1) * math.sin(2.0*math.pi*u2)))
    randNormal = mean + stdDev * randStdNormal
    #hacky way to construct a json object as a string
    #dataPoint = "{\"strike\":" + str(n) + ", \"time\":\"" + time.asctime() + "\", \"diode\":" + str(int(random.random()*12)) + ", \"energy\":" + str(randNormal*4) + ", \"bin:\"" + str(randNormal) + "}"
    dataPoint = json.dumps({"strike": n, "time": time.asctime(), "diode": int(random.random()*12), "energy": randNormal*4, "bin": randNormal});
    return dataPoint
 
 
if __name__ == "__main__":
    http_server = tornado.httpserver.HTTPServer(application)
    http_server.listen(8888)
    myIP = socket.gethostbyname(socket.gethostname())
    print '*** Websocket Server Started at %s***' % myIP
    tornado.ioloop.IOLoop.instance().start()
 
