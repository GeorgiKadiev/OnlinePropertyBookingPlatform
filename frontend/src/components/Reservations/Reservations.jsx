import React from "react";
import { MenuItem, MenuList, Divider, Box } from "@mui/material";
import { useSelector } from "react-redux";

const fetchReservations = async (url, token) => {
  try {
    const response = await fetch(url, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error("Failed to fetch reservations");
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching reservations:", error);
    return null;
  }
};

export default function MenuListComposition() {
  const role = useSelector((state) => state.role);
  const userId = useSelector((state) => state.id);
  const token = useSelector((state) => state.token);

  const handleGetReservations = async () => {
    let url;

    if (role === "Customer") {
      url = `http://localhost:5076/api/reservation/user-reservations/${userId}`;
    } else if (role === "EstateOwner") {
      const estateId = ""; // Replace with actual estateId logic
      url = `http://localhost:5076/api/estate/${estateId}/reservations`;
    } else {
      console.log("Invalid role");
      return;
    }

    const reservations = await fetchReservations(url, token);

    if (reservations) {
      console.log(`Reservations for ${role}:`, reservations);
    }
  };

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        [`& .MuiDivider-root`]: {
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
