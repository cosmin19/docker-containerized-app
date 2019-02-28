FROM node:latest

# Maintainer
LABEL maintainer="Cosmin Crisan" 

# Enviroment variables
ENV PORT=4200/tcp

# Set work directory
RUN mkdir -p /app
WORKDIR /app

# Install packages
COPY package.json /app/
RUN ["npm", "install"]

# Copy project files
COPY . /app

# Set port
# The EXPOSE instruction informs Docker that the container listens on the specified network ports at runtime.
EXPOSE $PORT

# Starting point
CMD ["npm", "start", "--", "--host", "0.0.0.0", "--poll", "500"]


# ################################## TERMINAL COMMANDS ###################################
# BUILD IMAGE: docker build -f Dockerfile -t enviroself-user .
# RUN CONTAINER (Specific port): docker run -d -it --rm -p 8080:4200 -v ${pwd}/src:/app/src enviroself-user

# ################################## AWS COMMANDS ###################################
# ssh -i "C:\Users\cosmi\.ssh\MyKeyPair.pem" ec2-user@52.25.180.105
