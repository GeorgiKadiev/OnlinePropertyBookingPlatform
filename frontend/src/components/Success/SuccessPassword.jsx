import React from "react";
import { Link } from "react-router-dom";

function SuccessPassword() {
  return (
    <div>
      <h1>Successful!</h1>
      <p>Now please check your email for link to change password.</p>
      <Link to="/login">Login</Link>
    </div>
  );
}

export default SuccessPassword;
