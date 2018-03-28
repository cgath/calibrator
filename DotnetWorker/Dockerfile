FROM microsoft/dotnet-nightly:2.1-runtime-alpine

# Install ZeroMQ and dependencies
RUN apk update && apk add zeromq libc6-compat libuuid libunwind

# Expose pool of ports to use
EXPOSE 4000-9999

# Add worker application
ADD published /tmp/published
