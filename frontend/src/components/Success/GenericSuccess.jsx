import React from "react";
import { Link } from "react-router-dom";

function Success() {
  return (
    <div>
      <h1>Successfull creating estate!</h1>
      <Link to="/landing-page">Home Page</Link>
    </div>
  );
}

export default Success;
