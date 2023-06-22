const express = require('express');
const fs = require('fs');

const app = express();

app.use(express.static('public'));
app.use(express.urlencoded({ extended: true }));

app.post('/write_file', (req, res) => {
  const { file, data } = req.body;

  if (file && data) {
    fs.writeFileSync(`./${file}`, data, 'utf8');
    res.send('ファイルに書き込みました。');
  } else {
    res.status(400).send('ファイル名とデータが提供されていません。');
  }
});

const port = 3001;
app.listen(port, '0.0.0.0', () => {
  console.log(`サーバーがポート ${port} で起動しました。`);
});
