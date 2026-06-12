const http = require('http');
const fs = require('fs');
const path = require('path');
const { spawn } = require('child_process');
const https = require('https');

const PORT = 8080;
const PUBLIC_DIR = __dirname;
const CONVO_ID = 'ad38060c-3de6-466e-afda-d435f9f52897';
const ARTIFACT_DIR = `C:/Users/user/.gemini/antigravity-ide/brain/${CONVO_ID}`;

// 1. Simple static file server
const server = http.createServer((req, res) => {
  let filePath = path.join(PUBLIC_DIR, req.url === '/' ? 'ar_index.html' : req.url);
  
  fs.readFile(filePath, (err, content) => {
    if (err) {
      res.writeHead(404, { 'Content-Type': 'text/plain' });
      res.end('File not found');
    } else {
      let ext = path.extname(filePath);
      let contentType = 'text/html';
      if (ext === '.js') contentType = 'text/javascript';
      else if (ext === '.css') contentType = 'text/css';
      
      res.writeHead(200, { 'Content-Type': contentType });
      res.end(content, 'utf-8');
    }
  });
});

server.listen(PORT, () => {
  console.log(`Local WebXR server running at http://localhost:${PORT}`);
  startTunnel();
});

// 2. Start SSH Tunnel and parse public URL
function startTunnel() {
  console.log('Starting SSH tunnel via localhost.run...');
  
  const ssh = spawn('ssh', [
    '-R', '80:localhost:8080',
    '-o', 'StrictHostKeyChecking=no',
    '-o', 'UserKnownHostsFile=NUL', // Windows equivalent of /dev/null
    'nokey@localhost.run'
  ]);

  let urlDetected = false;

  ssh.stdout.on('data', (data) => {
    const output = data.toString();
    console.log(`[SSH Tunnel] ${output.trim()}`);
    
    // Parse URL (e.g. https://xxxx.lhr.life or https://xxxx.lhr.rocks)
    const match = output.match(/https:\/\/[a-zA-Z0-9.-]+\.lh[a-zA-Z0-9.-]+/);
    if (match && !urlDetected) {
      const publicUrl = match[0];
      urlDetected = true;
      console.log(`\n🎉 Public URL detected: ${publicUrl}`);
      generateQRCode(publicUrl);
    }
  });

  ssh.stderr.on('data', (data) => {
    console.log(`[SSH Error/Info] ${data.toString().trim()}`);
  });

  ssh.on('close', (code) => {
    console.log(`SSH tunnel closed with code ${code}`);
    if (!urlDetected) {
      console.log('Retrying with pinggy...');
      startPinggyTunnel();
    }
  });
}

function startPinggyTunnel() {
  const ssh = spawn('ssh', [
    '-R', '80:localhost:8080',
    '-o', 'StrictHostKeyChecking=no',
    '-o', 'UserKnownHostsFile=NUL',
    'loop@ap.pinggy.io'
  ]);

  let urlDetected = false;

  ssh.stdout.on('data', (data) => {
    const output = data.toString();
    console.log(`[Pinggy] ${output.trim()}`);
    
    const match = output.match(/https:\/\/[a-zA-Z0-9.-]+\.pinggy\.link/);
    if (match && !urlDetected) {
      const publicUrl = match[0];
      urlDetected = true;
      console.log(`\n🎉 Public URL detected: ${publicUrl}`);
      generateQRCode(publicUrl);
    }
  });

  ssh.stderr.on('data', (data) => {
    console.log(`[Pinggy Error] ${data.toString().trim()}`);
  });
}

// 3. Generate QR Code image and save to artifacts
function generateQRCode(targetUrl) {
  const qrApiUrl = `https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=${encodeURIComponent(targetUrl)}`;
  const qrFilePath = path.join(ARTIFACT_DIR, 'qrcode.png');
  
  console.log(`Generating QR code for ${targetUrl}...`);
  console.log(`Saving QR code to: ${qrFilePath}`);

  const file = fs.createWriteStream(qrFilePath);
  https.get(qrApiUrl, (response) => {
    response.pipe(file);
    file.on('finish', () => {
      file.close();
      console.log('\n✅ QR Code successfully generated and saved!');
      console.log(`Embed Path: file:///${qrFilePath.replace(/\\/g, '/')}`);
    });
  }).on('error', (err) => {
    fs.unlink(qrFilePath, () => {});
    console.error(`Failed to generate QR Code: ${err.message}`);
  });
}
