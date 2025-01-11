import React, { useState } from "react";
import { MenuItem, MenuList, Divider, Box, Typography } from "@mui/material";
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

  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const handleGetReservations = async () => {
    setLoading(true);
    setErrorMessage(""); // Reset the error message
    let url;

    if (role === "Customer") {
      url = `http://localhost:5076/api/reservation/user-reservations/${userId}`;
    } else if (role === "EstateOwner") {
      const estateId = 15; // Replace with actual estateId logic
      url = `http://localhost:5076/api/estate/${estateId}/reservations`;
    } else {
      setLoading(false);
      setErrorMessage("Invalid role");
      return;
    }

    const reservationsData = await fetchReservations(url, token);

    if (reservationsData) {
      setReservations(reservationsData);
      console.log(reservationsData);
      reservationsData.map((reservation, index) => console.log(reservation.id));
    } else {
      setErrorMessage("No reservations found.");
    }

    setLoading(false);
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

      {/* Display Reservations */}
      {loading && <Typography variant="h6">Loading reservations...</Typography>}

      {errorMessage && (
        <Typography variant="h6" color="error">
          {errorMessage}
        </Typography>
      )}

      {!loading && !errorMessage && reservations.length === 0 && (
        <Typography variant="h6" color="textSecondary">
          No reservations available.
        </Typography>
      )}

      {!loading && !errorMessage && reservations.length > 0 && (
        <Box>
          {reservations.map((reservation, index) => (
            <Box key={index}>
              <Typography variant="body1">
                {reservation.customerName}
              </Typography>
              <Typography variant="body1">{reservation.checkInDate}</Typography>
              <Typography variant="body1">
                {reservation.checkOutDate}
              </Typography>
            </Box>
          ))}
        </Box>
      )}
    </Box>
  );
}
