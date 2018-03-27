import sys
import zmq
import time

def main(conn_str):
    #print(zmq.pyzmq_version())
    print(conn_str)

    context = zmq.Context()
    socket = context.socket(zmq.REP)
    #socket.bind("tcp://*:"+port)
    socket.bind(conn_str)

    print("Worker listening @ %s" % conn_str)

    while True:
        message = socket.recv()
        print("Received message: %s" % message)

        # Send response
        response = "Hello from Worker " + port
        socket.send(bytes(response))


if __name__ == "__main__":
    main(sys.argv[1])
