// import React, { useState } from "react";
import { Box, Button, IconButton, Avatar } from "@mui/material";
// import { faUser } from '@awesome.me/kit-KIT_CODE/icons/duotone/solid'

import { Home } from "@mui/icons-material";
import "./NavBar.css";

export default function NavBar() {
  return (
    <Box className="navbar">
      <Button
        startIcon={<Home />}
      >
        Home
      </Button>
      <div>
        <IconButton
          // onClick={handleClick}
          size="large"
          edge="end"
          color="inherit"
          aria-label="profile"
          // aria-controls={open ? "profile-menu" : undefined}
          aria-haspopup="true"
          // aria-expanded={open ? "true" : undefined}
        >
          <Avatar alt="Profile Picture" />
        </IconButton>
      </div>
    </Box>
  );
}
