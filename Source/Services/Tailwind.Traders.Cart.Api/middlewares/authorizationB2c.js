const passport = require("passport");

module.exports = () => {
    return passport.authenticate('oauth-bearer', { session: false });
};