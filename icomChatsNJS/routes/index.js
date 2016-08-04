var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function (req, res) {
    res.render('index', { title: 'Express' });
});

router.get('/mensaje', function (req, res) {
    res.render('mensaje', { title: 'Express' });
});

module.exports = router;