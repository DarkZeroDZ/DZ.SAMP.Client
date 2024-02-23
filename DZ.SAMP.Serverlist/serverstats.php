<?php

// Funktion, um Serverinformationen abzurufen
function getServerProperties($ip, $port) {
    // Dein bestehender Code für die Serverabfrage hier einfügen
    
    // Beispiel: Einfache Rückgabe mit Dummy-Daten
    $server = new stdClass();
    $server->IPAddress = $ip;
    $server->Port = $port;
    $server->Ping = rand(50, 200);
    $server->Players = rand(0, 50);
    $server->MaxPlayers = 50;
    $server->Name = "Mein Server";
    $server->Mode = "Freeroam";
    $server->Language = "DE";
    
    // Anfrage für Serverinfo ('i') senden
    $serverInfo = sendUDPRequest($ip, $port, 'i');

    // Teile die ServerInfo in Bereiche auf und schreibe alles in "hostname"
    $serverInfoArray = parseServerInfo($serverInfo);
    $server->hostname = implode(' ', $serverInfoArray);

    return $server;
}

// Funktion zum Parsen der ServerInfo-Daten ohne reguläre Ausdrücke
function parseServerInfo($data) {
    // Debug-Ausgabe der Daten vor der Verarbeitung
    echo "Debug: Raw Data Before Processing: $data\n";

    // Zerlege die Daten in Zeilen
    $lines = explode("\n", $data);

    // Array für die extrahierten Informationen
    $result = array();

    // Durchlaufe die Zeilen und suche nach Schlüsselwörtern
    foreach ($lines as $line) {
        // Zerlege die Zeile in Schlüssel und Wert
        list($key, $value) = explode(':', $line, 2);

        // Trimme Leerzeichen und füge sie zum Ergebnis hinzu
        $result[trim($key)] = trim($value);
    }

    // Debug-Ausgabe der Ergebnisse
    echo "Debug: Result After Processing: ";
    var_dump($result);

    return $result;
}


// Funktion, um UDP-Anfrage zu senden
function sendUDPRequest($ip, $port, $opCode) {
    $socket = socket_create(AF_INET, SOCK_DGRAM, SOL_UDP);
    
    if ($socket === false) {
        echo "Socket-Erstellung fehlgeschlagen: " . socket_strerror(socket_last_error()) . "\n";
        return null;
    }
    
    $message = "SAMP" . inet_pton($ip) . pack("n", $port) . $opCode;
    
    echo "Sending UDP request to $ip:$port with opcode $opCode\n";
    
    socket_sendto($socket, $message, strlen($message), 0, $ip, $port);
    
    socket_set_option($socket, SOL_SOCKET, SO_RCVTIMEO, array('sec' => 1, 'usec' => 0));
    
    $from = '';
    $port = 0;
    socket_recvfrom($socket, $buf, 2048, 0, $from, $port);
    
    socket_close($socket);
    
    echo "Received UDP response from $ip:$port\n";

    // Die ersten 11 Bytes überspringen
    $buf = substr($buf, 11);
    
    return $buf;
}

// Funktion zum Lesen der Serverliste aus der Textdatei
function readServerList($filename) {
    $servers = file($filename, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);
    return $servers;
}

function convertToSystemEncoding($data) {
    $convertedData = [];

    foreach ($data as $item) {
        $convertedItem = [];
        foreach ($item as $key => $value) {
            // Konvertiere ins System-Standardencoding
            $convertedItem[$key] = mb_convert_encoding($value, mb_internal_encoding(), 'UTF-8');
        }
        $convertedData[] = $convertedItem;
    }

    return $convertedData;
}

// Funktion zum Schreiben von Daten in eine JSON-Datei mit file_put_contents
function writeToJsonFile($filename, $data) {
    // Konvertiere die Daten ins System-Standardencoding
    $convertedData = convertToSystemEncoding($data);

    $jsonData = json_encode($convertedData, JSON_PRETTY_PRINT);
    
    // Debug-Ausgabe des Dateninhalts
    echo "Debug: Output Data:\n";
    var_dump($convertedData);

    // Überprüfe, ob json_encode einen Fehler gemeldet hat
    if (json_last_error() !== JSON_ERROR_NONE) {
        echo "Fehler beim Codieren der Daten in JSON: " . json_last_error_msg() . "\n";
    } else {
        // Versuche, die Daten in die Datei zu schreiben
        if (file_put_contents($filename, $jsonData, LOCK_EX) === false) {
            echo "Fehler beim Schreiben in die Datei: $filename\n";
        } else {
            echo "Daten erfolgreich in die Datei geschrieben: $filename\n";
        }
    }
}

// Funktion zum Ausgeben von Debug-Informationen
function debugLog($message) {
    echo $message . "\n";
}

// Hauptprogramm
if (isset($_GET['action']) && $_GET['action'] === 'update') {
    // Ausführung nur bei Aufruf mit ?action=update
    $serverListFile = "serverlist.txt";
    $outputFile = "output.json";
    
    debugLog("Skript gestartet: " . date('Y-m-d H:i:s'));

    $servers = readServerList($serverListFile);
    $outputData = [];

    foreach ($servers as $server) {
        list($ip, $port) = explode(":", $server);
        
        echo "Processing server $ip:$port\n";

        // Serverinformationen abrufen
        $serverInfo = getServerProperties($ip, $port);

        // Hier kannst du die erhaltenen Daten in einem gewünschten Format zusammensetzen
        $outputData[] = [
            'Server' => "$ip:$port",
            'ServerInfo' => $serverInfo,
        ];
    }

    // Daten in die Ausgabedatei schreiben
    var_dump($outputData); // Hier hinzugefügt
    writeToJsonFile($outputFile, $outputData);

    debugLog("Skript beendet: " . date('Y-m-d H:i:s'));
} else {
    // Ausgabe für den Browser
    echo '<p>Besuche die URL mit ?action=update, um das Skript auszuführen.</p>';
}
?>