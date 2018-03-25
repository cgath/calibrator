FROM rcarmo/alpine-python

RUN apk update && apk add zeromq
RUN pip install pyzmq

ADD worker.py /tmp/worker.py

EXPOSE 5555

CMD ["python","/tmp/worker.py"]
