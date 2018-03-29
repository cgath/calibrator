FROM rcarmo/alpine-python

RUN apk update && apk add zeromq
RUN pip install pyzmq

EXPOSE 4000-9999

ADD worker.py /tmp/worker.py
