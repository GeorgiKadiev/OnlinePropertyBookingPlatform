import React from "react";
import {  Link } from "react-router-dom";

function SuccessPage() {
  return (
    <div>
      <h1>Registration Successful!</h1>
      <p>Thank you for registering. Now please confirm your email.</p>
      <Link to="/login">Login</Link>
    </div>
  );
}

export default SuccessPage;