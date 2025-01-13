import React, { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { useParams } from "react-router-dom";
import {
  MenuItem,
  MenuList,
  Divider,
  Box,
  Typography,
  Button,
  TextField,
  Rating,
} from "@mui/material";
import dayjs from "dayjs";
import "./Reservations.css"; // Import CSS file

export default function Reservations() {
  const role = useSelector((state) => state.role);
  const userId = useSelector((state) => state.id);
  const token = useSelector((state) => state.token);

  // if (role === "EstateOwner") 
    const { id } = useParams();
  console.log(id)

  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [filteredReservations, setFilteredReservations] = useState([]);
  const [filterType, setFilterType] = useState("All");
  const [review, setReview] = useState({ rating: 0, comment: "" });
  const [loadingReview, setLoadingReview] = useState(false);

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
    } catch (error) {
      console.error("Error posting review:", error);
      return false;
    }
  };

  const handleGetReservations = async () => {
    setLoading(true);
    setErrorMessage(""); // Reset the error message
    let url;

    if (role === "Customer") {
      url = `http://localhost:5076/api/reservation/user-reservations/${userId}`;
    } else if (role === "EstateOwner") {
      console.log(id);
      url = `http://localhost:5076/api/estate/${id}/reservations`;
      console.log(url);
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

  const handleCancelReservation = async (reservationId) => {
    try {
      const response = await fetch(
        `http://localhost:5076/api/reservation/delete/${reservationId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to cancel the reservation");
      }

      alert("Reservation canceled successfully!");

      setReservations((prevReservations) =>
        prevReservations.filter(
          (reservation) => reservation.id !== reservationId
        )
      );
      filterReservations(
        reservations.filter((reservation) => reservation.id !== reservationId),
        filterType
      );
    } catch (error) {
      console.error("Error canceling reservation:", error);
      alert("Failed to cancel reservation. Please try again.");
    }
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
    if (loadingReview) return;

    setLoadingReview(true);
    const response = await postReview(
      estateId,
      review.rating,
      review.comment,
      token
    );

    if (!response) {
      alert("Review posted successfully!");
      setReview({ rating: 0, comment: "" });
    }
    setLoadingReview(false);
  };

  useEffect(() => {
    handleGetReservations();
  }, []);

  return (
    <Box className="main-container">
      <MenuList className="menu-list">
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

      {loading && <Typography className="loading-text">Loading...</Typography>}

      {errorMessage && (
        <Typography className="error-text">{errorMessage}</Typography>
      )}

      {!loading && !errorMessage && filteredReservations.length === 0 && (
        <Typography className="no-reservations">
          No reservations available.
        </Typography>
      )}

      {!loading && !errorMessage && filteredReservations.length > 0 && (
        <Box sx={{ padding: 2, width: "-webkit-fill-available" }}>
          {filteredReservations.map((reservation) => {
            const isCancelable =
              dayjs(reservation.checkInDate).isAfter(dayjs()) &&
              dayjs(reservation.checkInDate).diff(dayjs(), "day") >= 2;

            return (
              <Box key={reservation.id} className="reservation-card">
                <Typography className="reservation-title">
                  {reservation.estateName}
                </Typography>
                <Divider className="reservation-divider" />
                <Typography className="reservation-info">
                  <strong>Customer:</strong> {reservation.customerName}
                </Typography>
                <Typography className="reservation-info">
                  <strong>Check-in Date:</strong> {reservation.checkInDate}
                </Typography>
                <Typography className="reservation-info">
                  <strong>Check-out Date:</strong> {reservation.checkOutDate}
                </Typography>
                <Typography className="reservation-info">
                  <strong>Total Price:</strong>{" "}
                  {reservation.totalPrice || "Not available"}
                </Typography>

                {isCancelable && (
                  <Button
                    variant="contained"
                    color="error"
                    className="cancel-button"
                    onClick={() => handleCancelReservation(reservation.id)}
                  >
                    Cancel Reservation
                  </Button>
                )}

                {dayjs(reservation.checkOutDate).isBefore(dayjs()) &&
                  role === "Customer" && (
                    <Box className="review-section">
                      <Typography className="review-section-title">
                        Post a Review
                      </Typography>
                      <Box className="review-inputs">
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
                        />
                        <Button
                          variant="contained"
                          color="primary"
                          className="submit-review-button"
                          onClick={() => handlePostReview(reservation.estateId)}
                          disabled={loadingReview}
                        >
                          {loadingReview ? "Posting..." : "Submit Review"}
                        </Button>
                      </Box>
                    </Box>
                  )}
              </Box>
            );
          })}
        </Box>
      )}
    </Box>
  );
}
