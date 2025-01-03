import React from "react";
import {  Link } from "react-router-dom";

function SuccessRegister() {
  return (
    <div>
      <h1>Registration Successful!</h1>
      <p>Thank you for registering. Now please confirm your email.</p>
      <Link to="/login">Login</Link>
    </div>
  );
}

export default SuccessRegister;