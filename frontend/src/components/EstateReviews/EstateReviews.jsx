import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Rating,
  CircularProgress,
  List,
  ListItem,
  ListItemText,
  IconButton,
  Snackbar,
  Alert,
} from "@mui/material";
import FlagIcon from "@mui/icons-material/Flag"; // Import FlagIcon
import { useSelector } from "react-redux"; // To get the role from Redux store
import "./EstateReviews.css";

export default function Reviews() {
  const { id } = useParams(); // Get the property ID from URL parameter
  const role = useSelector((state) => state.role); // Get the user's role
  const token = useSelector((state) => state.token);

  const [reviews, setReviews] = useState([]); // State to store reviews
  const [loading, setLoading] = useState(true); // State to handle loading state
  const [error, setError] = useState(null); // State to handle errors during fetching
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success",
  });

  useEffect(() => {
    const fetchReviews = async () => {
      try {
        const response = await fetch(
          `http://localhost:5076/api/review/estate-reviews/${id}`
        );
        if (!response.ok) {
          throw new Error("Failed to fetch reviews");
        }
        const data = await response.json();
        setReviews(data);
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchReviews();
  }, [id]);

  const handleFlagClick = async (reviewId) => {
    try {
      const response = await fetch(
        `http://localhost:5076/api/review/flag/${reviewId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`, // Include the token for authentication
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to flag the review");
      }

      // Show success message
      setSnackbar({
        open: true,
        message:
          "The review has been flagged successfully and will be reviewed by the admin.",
        severity: "success",
      });
    } catch (error) {
      console.error("Error flagging review:", error);
      // Show error message
      setSnackbar({
        open: true,
        message: "Failed to flag the review. Please try again.",
        severity: "error",
      });
    }
  };

  const handleCloseSnackbar = () => {
    setSnackbar({ ...snackbar, open: false });
  };

  if (loading) {
    return (
      <Box className="reviews-loading">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box className="reviews-error">
        <Typography color="error">Error: {error}</Typography>
      </Box>
    );
  }

  return (
    <Box className="reviews-container">
      <Typography variant="h5" className="reviews-title">
        Reviews
      </Typography>
      {reviews.length > 0 ? (
        <List className="reviews-list">
          {reviews.map((review) => (
            <Box className="review-item-container" key={review.id}>
              <ListItem className="review-item">
                <ListItemText secondary={review.comment} />
                {/* Conditionally render the flag button */}
                {role === "EstateOwner" && (
                  <IconButton
                    color="error"
                    onClick={() => handleFlagClick(review.id)}
                  >
                    <FlagIcon />
                  </IconButton>
                )}
              </ListItem>
              <Box sx={{ display: "flex", flexDirection: "column" }}>
                <Box sx={{ "& > legend": { mt: 2 } }} className="review-rating">
                  <Typography component="legend">Rating</Typography>
                  <Rating name="read-only" value={review.rating} readOnly />
                </Box>
                <ListItem>
                  <ListItemText secondary={review.date} />
                </ListItem>
              </Box>
            </Box>
          ))}
        </List>
      ) : (
        <Box className="no-reviews-container">
          <Typography variant="h6" className="no-reviews-message">
            No reviews available for this property.
          </Typography>
        </Box>
      )}

      {/* Snackbar for feedback */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={handleCloseSnackbar}
          severity={snackbar.severity}
          sx={{ width: "100%" }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
