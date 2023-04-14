import socket

from proto import HeaderFrame, Frame, NnOutput, MyVector2

# open server on port 6969
server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(("localhost", 6969))

while True:
    server.listen(1)
    print("Waiting for connection...")
    conn, addr = server.accept()
    with conn:
        print("Connected by", addr)

        while True:
            data = conn.recv(1024)
            if not data:
                print("no data")
                break

            frame = Frame.FromString(data)
            print(frame)

            output = NnOutput()
            output.desired_move_direction = MyVector2(x=0, y=1)

            conn.sendall(bytes(output))
