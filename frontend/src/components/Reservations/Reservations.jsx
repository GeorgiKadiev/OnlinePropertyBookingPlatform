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
    } else {
      setErrorMessage("No reservations found.");
    }

    setLoading(false);
  };

  // Load reservations automatically when the component is mounted
  React.useEffect(() => {
    handleGetReservations(); // Automatically fetch reservations on mount
  }, []);

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "flex-start",
        [`& .MuiDivider-root`]: {
          mx: 0.5,
        },
      }}
    >
      <MenuList>
        {/* "All reservations" button */}
        <MenuItem onClick={handleGetReservations}>All reservations</MenuItem>
        <MenuItem>Current reservations</MenuItem>
        <MenuItem>Past reservations</MenuItem>
        <MenuItem>Future reservations</MenuItem>
      </MenuList>
      <Divider orientation="vertical" flexItem />

      {/* Display Reservations or Loading/Error Message */}
      {loading && <Typography variant="h6">Loading reservations...</Typography>}

      {errorMessage && (
        <Typography variant="h6" color="error">
          {errorMessage}
        </Typography>
      )}

      {/* No reservations found message */}
      {!loading && !errorMessage && reservations.length === 0 && (
        <Typography variant="h6" color="textSecondary">
          No reservations available.
        </Typography>
      )}

      {/* Display actual reservations */}
      {!loading && !errorMessage && reservations.length > 0 && (
        <Box sx={{ padding: 2 }}>
          {reservations.map((reservation) => (
            <Box
              key={reservation.id}
              sx={{
                border: "1px solid #ddd", // Light border around each reservation
                borderRadius: "8px", // Rounded corners
                padding: "16px",
                marginBottom: "16px", // Space between reservations
                boxShadow: 1, // Slight shadow for a card-like feel
              }}
            >
              <Typography variant="h6" sx={{ fontWeight: "bold" }}>
                {reservation.estateName}
              </Typography>
              <Divider sx={{ margin: "8px 0" }} />
              <Typography variant="body1">
                <strong>Customer:</strong> {reservation.customerName}
              </Typography>
              <Typography variant="body1">
                <strong>Check-in Date:</strong> {reservation.checkInDate}
              </Typography>
              <Typography variant="body1">
                <strong>Check-out Date:</strong> {reservation.checkOutDate}
              </Typography>
              <Typography variant="body1">
                <strong>Total Price:</strong>{" "}
                {reservation.totalPrice || "Not available"}
              </Typography>
              <Typography variant="body2" color="textSecondary">
                <strong>Status:</strong> {reservation.status || "Pending"}
              </Typography>
            </Box>
          ))}
        </Box>
      )}
    </Box>
  );
}
