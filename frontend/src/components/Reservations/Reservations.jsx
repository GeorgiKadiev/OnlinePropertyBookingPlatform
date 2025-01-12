import React, { useState, useEffect } from "react";
import {
  MenuItem,
  MenuList,
  Divider,
  Box,
  Typography,
  Button,
  TextField,
} from "@mui/material";
import { useSelector } from "react-redux";
import dayjs from "dayjs"; // Import dayjs for date comparison
import Rating from "@mui/material/Rating";

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

const postReview = async (estateId, rating, comment, token) => {
  try {
    const payload = JSON.stringify({
      Rating: rating,
      Comment: comment,
    });

    console.log("Payload being sent:", payload);
    console.log("Estate ID:", estateId);

    const response = await fetch(
      `http://localhost:5076/api/review/create/${estateId}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: payload,
      }
    );

    // Check for response status and handle empty responses
    if (!response.ok) {
      throw new Error("Failed to post review");
    }
  } catch (error) {
    console.error("Error posting review:", error);
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
  const [filteredReservations, setFilteredReservations] = useState([]);
  const [filterType, setFilterType] = useState("All");
  const [review, setReview] = useState({ rating: 0, comment: "" });
  const [loadingReview, setLoadingReview] = useState(false);

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
      filterReservations(reservationsData, filterType);
    } else {
      setErrorMessage("No reservations found.");
    }

    setLoading(false);
  };

  const filterReservations = (reservationsData, filterType) => {
    const currentDate = dayjs(); // Get current date using dayjs
    let filtered = [];

    switch (filterType) {
      case "Current":
        filtered = reservationsData.filter(
          (reservation) =>
            dayjs(reservation.checkInDate).isBefore(currentDate) &&
            dayjs(reservation.checkOutDate).isAfter(currentDate)
        );
        break;
      case "Past":
        filtered = reservationsData.filter((reservation) =>
          dayjs(reservation.checkOutDate).isBefore(currentDate)
        );
        break;
      case "Future":
        filtered = reservationsData.filter((reservation) =>
          dayjs(reservation.checkInDate).isAfter(currentDate)
        );
        break;
      default:
        filtered = reservationsData;
        break;
    }

    setFilteredReservations(filtered);
  };

  const handleFilterChange = (filter) => {
    setFilterType(filter);
    filterReservations(reservations, filter);
  };

  const handleReviewChange = (e) => {
    setReview({
      ...review,
      [e.target.name]: e.target.value,
    });
  };

  const handlePostReview = async (estateId) => {
    if (loadingReview) return; // Prevent multiple submissions

    setLoadingReview(true);
    const response = await postReview(
      estateId,
      review.rating,
      review.comment,
      token
    );

    if (response) {
      alert("Review posted successfully!");
      setReview({ rating: 0, comment: "" }); // Reset review form
    } else {
      alert("Failed to post review. Please try again.");
    }
    setLoadingReview(false);
  };

  // Load reservations automatically when the component is mounted
  useEffect(() => {
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
        <MenuItem onClick={() => handleFilterChange("All")}>
          All reservations
        </MenuItem>
        <MenuItem onClick={() => handleFilterChange("Current")}>
          Current reservations
        </MenuItem>
        <MenuItem onClick={() => handleFilterChange("Past")}>
          Past reservations
        </MenuItem>
        <MenuItem onClick={() => handleFilterChange("Future")}>
          Future reservations
        </MenuItem>
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
      {!loading && !errorMessage && filteredReservations.length === 0 && (
        <Typography variant="h6" color="textSecondary">
          No reservations available.
        </Typography>
      )}

      {/* Display actual reservations */}
      {!loading && !errorMessage && filteredReservations.length > 0 && (
        <Box sx={{ padding: 2 }}>
          {filteredReservations.map((reservation) => (
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

              {/* Review section (only for past reservations) */}
              {dayjs(reservation.checkOutDate).isBefore(dayjs()) && (
                <Box sx={{ marginTop: 2 }}>
                  <Typography variant="h6">Post a Review</Typography>
                  <Box sx={{ display: "flex", flexDirection: "column" }}>
                    <Rating
                      name="rating"
                      value={review.rating}
                      onChange={(e, newValue) =>
                        setReview({ ...review, rating: newValue })
                      }
                    />
                    <TextField
                      label="Comment"
                      name="comment"
                      multiline
                      rows={4}
                      value={review.comment}
                      onChange={handleReviewChange}
                      sx={{ marginTop: 2 }}
                    />
                    <Button
                      variant="contained"
                      color="primary"
                      sx={{ marginTop: 2 }}
                      onClick={() => handlePostReview(reservation.estateId)}
                      disabled={loadingReview}
                    >
                      {loadingReview ? "Posting..." : "Submit Review"}
                    </Button>
                  </Box>
                </Box>
              )}
            </Box>
          ))}
        </Box>
      )}
    </Box>
  );
}
