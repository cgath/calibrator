import zmq
import time

print(zmq.pyzmq_version())

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

print("Worker listening @ tcp://*:5555")

while True:
    message = socket.recv()
    print("Received message: %s" % message)

    # Send response
    socket.send(b"Hello from worker!")
