import * as React from "react";
import MenuItem from "@mui/material/MenuItem";
import MenuList from "@mui/material/MenuList";
import Divider, { dividerClasses } from "@mui/material/Divider";
import Box from "@mui/material/Box";
import { useSelector } from "react-redux"; 

export default function MenuListComposition() {
  const role = useSelector((state) => state.role); 
  const userId = useSelector((state) => state.id); 
  const token = useSelector((state) => state.token);

  const handleGetReservations = async () => {
    if (role === "Customer") {
      try {
        const response = await fetch(
          `http://localhost:5076/api/reservation/user-reservations/${userId}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`, 
            },
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch reservations");
        }

        const reservations = await response.json();
        console.log("Reservations for Customer:", reservations);
        
      } catch (error) {
        console.error("Error fetching reservations:", error);
      }
    } else if (role === "EstateOwner") {
      console.log("No action for EstateOwner yet");
    } else {
      console.log("Invalid role");
    }
  };

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        [`& .${dividerClasses.root}`]: {
          mx: 0.5,
        },
      }}
    >
      <MenuList>
        <MenuItem onClick={handleGetReservations}>All reservations</MenuItem>
        <MenuItem>Current reservations</MenuItem>
        <MenuItem>Past reservations</MenuItem>
        <MenuItem>Future reservations</MenuItem>
      </MenuList>
      <Divider orientation="vertical" flexItem />
    </Box>
  );
}
