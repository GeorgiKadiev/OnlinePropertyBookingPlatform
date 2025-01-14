import React, { useState, useEffect } from "react";
import {
  Box,
  Typography,
  CircularProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Snackbar,
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";
import { useSelector } from "react-redux";
import DeleteIcon from "@mui/icons-material/Delete";
import NavBar from "../../../components/NavBar/NavBar";

export default function AdminDashboard() {
  const token = useSelector((state) => state.token); // Get the token from Redux store
  const [reviews, setReviews] = useState([]); // State to store flagged reviews
  const [loading, setLoading] = useState(true); // Loading state
  const [error, setError] = useState(null); // Error state
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success",
  }); // Snackbar state
  const [deleteDialog, setDeleteDialog] = useState({
    open: false,
    reviewId: null,
  }); // Delete confirmation dialog

  useEffect(() => {
    const fetchFlaggedReviews = async () => {
      try {
        const response = await fetch(
          "http://localhost:5076/api/review/flagged",
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch flagged reviews");
        }

        const data = await response.json();
        setReviews(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchFlaggedReviews();
  }, [token]);

  const handleDeleteReview = async (reviewId) => {
    try {
      const response = await fetch(
        `http://localhost:5076/api/review/${reviewId}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to delete the review");
      }

      // Remove the deleted review from the state
      setReviews((prevReviews) =>
        prevReviews.filter((review) => review.id !== reviewId)
      );

      // Show success snackbar
      setSnackbar({
        open: true,
        message: "Review deleted successfully.",
        severity: "success",
      });
    } catch (err) {
      console.error("Error deleting review:", err);
      setSnackbar({
        open: true,
        message: "Failed to delete the review. Please try again.",
        severity: "error",
      });
    }
  };

  const handleOpenDeleteDialog = (reviewId) => {
    setDeleteDialog({ open: true, reviewId });
  };

  const handleCloseDeleteDialog = () => {
    setDeleteDialog({ open: false, reviewId: null });
  };

  const handleConfirmDelete = () => {
    if (deleteDialog.reviewId) {
      handleDeleteReview(deleteDialog.reviewId);
    }
    handleCloseDeleteDialog();
  };

  const handleCloseSnackbar = () => {
    setSnackbar({ ...snackbar, open: false });
  };

  return (
    <div>
      <NavBar />
      <Box sx={{ padding: 3 }}>
        <Typography variant="h4" gutterBottom>
          Admin Dashboard
        </Typography>
        <Typography variant="h6" gutterBottom>
          Flagged Reviews
        </Typography>

        {loading ? (
          <CircularProgress />
        ) : error ? (
          <Typography color="error">{error}</Typography>
        ) : reviews.length === 0 ? (
          <Typography variant="body1">No flagged reviews available.</Typography>
        ) : (
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>
                    <strong>Review ID</strong>
                  </TableCell>
                  <TableCell>
                    <strong>Comment</strong>
                  </TableCell>
                  <TableCell>
                    <strong>Rating</strong>
                  </TableCell>
                  <TableCell>
                    <strong>Date</strong>
                  </TableCell>
                  <TableCell>
                    <strong>Actions</strong>
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {reviews.map((review) => (
                  <TableRow key={review.id}>
                    <TableCell>{review.id}</TableCell>
                    <TableCell>{review.comment}</TableCell>
                    <TableCell>{review.rating}</TableCell>
                    <TableCell>{review.date}</TableCell>
                    <TableCell>
                      {/* Delete Button */}
                      <IconButton
                        color="error"
                        onClick={() => handleOpenDeleteDialog(review.id)}
                      >
                        <DeleteIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}

        {/* Delete Confirmation Dialog */}
        <Dialog open={deleteDialog.open} onClose={handleCloseDeleteDialog}>
          <DialogTitle>Delete Review</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Are you sure you want to delete this review? This action cannot be
              undone.
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDeleteDialog} color="primary">
              Cancel
            </Button>
            <Button onClick={handleConfirmDelete} color="error">
              Delete
            </Button>
          </DialogActions>
        </Dialog>

        {/* Snackbar for notifications */}
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
    </div>
  );
}
