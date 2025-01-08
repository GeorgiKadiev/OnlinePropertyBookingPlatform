import React, { useState } from "react";
import { Home } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { clearToken } from "../../utils/tokenSlice";
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
import "./NavBar.css";
import { clearRole } from "../../utils/roleSlice";

export default function NavBar() {
  const rgbColor = "rgb(88, 19, 93)";
  const token = useSelector((state) => state.token);
  const userRole = useSelector((state) => state.role);
  console.log(userRole);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const [state, setState] = useState({
    right: false,
  });

  const menuItems = [
    { text: "View Reservations", showFor: "Customer" },
    { text: "Reset Password", showFor: "All" },
    { text: "Log Out", showFor: "All" },
  ];

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
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error("Failed to log out");
      }

      console.log("Logged out successfully");
      dispatch(clearToken());
      dispatch(clearRole());
      setState({ right: false });
      navigate("/");
    } catch (error) {
      console.error("Error logging out:", error);
      alert("Error logging out. Please try again.");
    }
  };

  const handleMenuClick = (option) => {
    switch (option) {
      case "View Reservations":
        navigate("/reservations");
        console.log("Navigating to Reservations...");
        break;
      case "Reset Password":
        console.log("Navigating to Reset Password...");
        break;
      case "Log Out":
        handleLogout();
        break;
      default:
        break;
    }
    setState({ right: false });
  };

  const handleHome = () => {
    navigate("/landing-page");
  };

  const list = () => (
    <Box
      role="presentation"
      onClick={toggleDrawer("right", false)}
      onKeyDown={toggleDrawer("right", false)}
    >
      <List>
        {menuItems
          .filter(
            (item) => item.showFor === "All" || item.showFor === userRole // Filter based on user type
          )
          .map((item) => (
            <ListItem key={item.text} disablePadding>
              <ListItemButton onClick={() => handleMenuClick(item.text)}>
                <ListItemText primary={item.text} />
              </ListItemButton>
            </ListItem>
          ))}
      </List>
    </Box>
  );

  return (
    <Box className="navbar">
      <Button
        startIcon={<Home sx={{ color: rgbColor }} />}
        sx={{ color: rgbColor }}
        onClick={handleHome}
      >
        Home
      </Button>
      <div>
        <IconButton
          onClick={toggleDrawer("right", true)}
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
