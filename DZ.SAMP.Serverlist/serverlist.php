<?php
$accessKey = 'Y8MIh9lgaD8FivYqJf22phxexr3RW9vv';

function addToServerList($data) {
    $file = 'serverlist.txt';

    if (!file_exists($file)) {
        file_put_contents($file, '');
    }

    $lines = file($file, FILE_IGNORE_NEW_LINES);

    if (preg_match('/^\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5}\b$/', $data)) {
        $oldserver = $data; // Füge einen Zeilenumbruch hinzu
        $data = $data . PHP_EOL; // Füge einen Zeilenumbruch hinzu
        if (!in_array($oldserver, $lines)) {
            file_put_contents($file, $data, FILE_APPEND); // Hinzufügen ohne Zeilenumbruch
        } else {
            echo "Server already exists.";
        }
    } else {
        echo "Invalid data format. Use IPv4:Port format.";
    }
}

function createUdpMessage($ip, $port) {
    $ipBytes = explode('.', $ip);
    $ipPacked = pack('C4', $ipBytes[0], $ipBytes[1], $ipBytes[2], $ipBytes[3]);

    $portPacked = pack('v', $port);

    $message = 'SAMP' . $ipPacked . $portPacked . 'i';

    return $message;
}

function checkIfServersAreAvailable() {
    $file = 'serverlist.txt';

    if (file_exists($file)) {
        $lines = file($file, FILE_IGNORE_NEW_LINES);

        foreach ($lines as $index => $line) {
            list($ip, $port) = explode(':', $line);

            $socket = socket_create(AF_INET, SOCK_DGRAM, SOL_UDP);
            socket_set_nonblock($socket);

            $timeout = 2;

            $message = createUdpMessage($ip, $port);

            $start = time();
            $result = @socket_sendto($socket, $message, strlen($message), 0, $ip, $port);

            if ($result !== false) {
                $response = '';
                $recvStartTime = time();

                while (time() - $recvStartTime < $timeout) {
                    if (socket_recvfrom($socket, $recvData, 4096, 0, $ip, $port)) {
                        $response .= $recvData;
                        break;
                    }
                }

                if (empty($response)) {
                    echo "Warning: Server $ip:$port is not reachable." . PHP_EOL;
                    unset($lines[$index]);
                }
            }

            socket_close($socket);
        }

        // Stelle sicher, dass am Ende der Datei ein Zeilenumbruch ist
        $lines[] = '';

        // Speichere die aktualisierte Liste in die Datei
        file_put_contents($file, implode(PHP_EOL, $lines));
    }
}

if (isset($_GET['accessKey']) && $_GET['accessKey'] === $accessKey) {
    if (isset($_GET['server'])) {
        $serverData = $_GET['server'];

        if (preg_match('/^\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5}\b$/', $serverData)) {
            addToServerList($serverData);

            checkIfServersAreAvailable();
        } else {
            echo "Invalid data format. Use IPv4:Port format.";
        }
    } else {
        echo "No IP address given.";
    }
} else {
    echo "Bad access key.";
}
?>