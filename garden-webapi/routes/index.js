var express = require('express');
var router = express.Router();
var gpio = require('../gpio.js');

const irrigationGpioPin = 2;
var isIrrigationOn = false;

function startIrrigation() {
  isIrrigationOn = true;
  gpio(irrigationGpioPin, 'low');
}
function stopIrrigation() {
  gpio(irrigationGpioPin, 'high');
  isIrrigationOn = false;
}

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', { title: 'Express' });
});

router.get('/pin/:id/:state', function (req, res, next) {
  gpio(req.params.id, req.params.state);
});

router.post('/on/:seconds*?', function (req, res, next) {
  // if the user doesn't specify a time to keep irrigation 
  // on then default to 5 minutes
  const seconds = req.params.seconds || 5 * 60;
  
  startIrrigation();
  res.send(200);
  setTimeout(stopIrrigation, seconds * 1000);
});

router.post('/off', function (req, res, next) {
  stopIrrigation();
  res.send(200);
});

router.get('/status', function (req, res, next) {
  res.status(200).json(isIrrigationOn);
});

module.exports = router;
