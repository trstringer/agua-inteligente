var exec = require('child_process').exec;

module.exports = (pin, state) => {
  if (state.toLowerCase() === 'low') {
    exec('gpio-setpinlow.py ' + pin);
  }
  else if (state.toLowerCase() === 'high') {
    exec('gpio-setpinhigh.py ' + pin);
  }
};