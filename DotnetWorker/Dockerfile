FROM microsoft/dotnet-nightly:2.1-runtime-alpine

RUN apk update && apk add zeromq libc6-compat libuuid libunwind

ADD published /tmp/published
