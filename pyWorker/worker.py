import zmq
import time

def main(port):
    #print(zmq.pyzmq_version())

    context = zmq.Context()
    socket = context.socket(zmq.REP)
    socket.bind("tcp://*:"+port)

    #print("Worker listening @ tcp://*:5555")

    while True:
        message = socket.recv()
        print("Received message: %s" % message)

        # Send response
        response = "Hello from Worker " + port
        socket.send(bresponse)


if __name__ == "__main__"
    main()
