import React, { useState } from "react";
import {
  Box,
  IconButton,
  Avatar,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Button,
} from "@mui/material";
import { Home } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import "./NavBar.css";

export default function NavBar({token}) {
  const navigate = useNavigate();
  const [state, setState] = useState({
    right: false,
  });

  const toggleDrawer = (anchor, open) => (event) => {
    if (
      event.type === "keydown" &&
      (event.key === "Tab" || event.key === "Shift")
    ) {
      return;
    }

    setState({ ...state, [anchor]: open });
  };

  const handleLogout = async () => {
    try {
      const response = await fetch("http://localhost:5076/api/user/logout", {
        method: "POST",
        headers: {
          "Authorization": `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Failed to log out");
      }

      console.log("Logged out successfully");
      // alert("Logged out successfully!");
      setState({ right: false });
      navigate("/success");
    } catch (error) {
      console.error("Error logging out:", error);
      alert("Error logging out. Please try again.");
    }
  };

  const handleMenuClick = (option) => {
    switch (option) {
      case "Forgot Password":
        console.log("Navigating to Forgot Password...");
        break;
      case "Reset Password":
        console.log("Navigating to Reset Password...");
        break;
      case "Log Out":
        handleLogout(); // Call the logout function
        break;
      default:
        break;
    }
    setState({ right: false }); // Close the drawer after an action
  };

  const list = () => (
    <Box
      role="presentation"
      onClick={toggleDrawer("right", false)}
      onKeyDown={toggleDrawer("right", false)}
    >
      <List>
        {["Forgot Password", "Reset Password", "Log Out"].map((text) => (
          <ListItem key={text} disablePadding>
            <ListItemButton onClick={() => handleMenuClick(text)}>
              <ListItemText primary={text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  );

  return (
    <Box className="navbar">
      <Button startIcon={<Home />}>Home</Button>
      <div>
        <IconButton
          onClick={toggleDrawer("right", true)} // Open "right" drawer on click
          size="large"
          edge="end"
          color="inherit"
          aria-label="profile"
        >
          <Avatar alt="Profile Picture" />
        </IconButton>
        <Drawer
          anchor={"right"}
          open={state["right"]}
          onClose={toggleDrawer("right", false)}
        >
          {list()}
        </Drawer>
      </div>
    </Box>
  );
}
